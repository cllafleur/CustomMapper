using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib
{
    public class MapperCompiler<TOrigin, TTarget>
    {
        private IFunctionHandlerProvider _functionHandlerProvider;
        private readonly CompilerOptions options;
        private readonly IPropertyResolverHandler sourcePropertyHandler;
        private readonly IPropertyResolverHandler targetPropertyHandler;

        public MapperCompiler(IFunctionHandlerProvider functionHandlerProvider, CompilerOptions options)
            : this(functionHandlerProvider, options, null, null)
        {
        }

        public MapperCompiler(IFunctionHandlerProvider functionHandlerProvider,
             CompilerOptions options,
             IPropertyResolverHandler sourcePropertyHandler = null,
             IPropertyResolverHandler targetPropertyHandler = null)
        {
            _functionHandlerProvider = functionHandlerProvider;
            this.options = options;
            this.sourcePropertyHandler = sourcePropertyHandler ?? DefaultPropertyResolverHandler.Instance;
            this.targetPropertyHandler = targetPropertyHandler ?? DefaultPropertyResolverHandler.Instance;
        }

        public IMapperHandler<TOrigin, TTarget> Compile(IEnumerable<StatementMapper> statements)
        {
            List<IStatementRuntimeHandler<TOrigin, TTarget>> operations = new List<IStatementRuntimeHandler<TOrigin, TTarget>>();
            foreach (var statement in statements)
            {
                operations.Add(BuildStatementRuntime(statement));
            }
            return new MapperHandler<TOrigin, TTarget>(operations);
        }

        private IStatementRuntimeHandler<TOrigin, TTarget> BuildStatementRuntime(StatementMapper statement)
        {
            var getRuntimeHandler = BuildGetRuntimeHandler<TOrigin>(statement.OriginExpr);
            var setRuntimeHandler = BuildSetRuntimeHandler<TTarget>(statement.TargetExpr);
            return new StatementRuntimeHandler<TOrigin, TTarget>(getRuntimeHandler, setRuntimeHandler, statement.ParsingInfo);
        }

        private IGetRuntimeHandler<T> BuildGetRuntimeHandler<T>(IExpressionMapper expression)
        {
            string expressionName = (expression as INamedExpressionMapper)?.ExpressionName;
            switch (expression)
            {
                case InstanceRefMapper instanceRef:
                    IGetterAccessor instanceVisitor;
                    if (options.Version == CompilerVersions.v2)
                    {
                        instanceVisitor = BuildGetInstanceVisitor<T>(instanceRef, sourcePropertyHandler);
                    }
                    else
                    {
                        instanceVisitor = BuildInstanceVisitor<T>(instanceRef, sourcePropertyHandler);
                    }
                    return new GetRuntimeHandler<T>(instanceVisitor, instanceRef.ParsingInfo, expressionName);
                case TextMapper textMapper:
                    return new TextGetRuntimeHandler<T>(textMapper.Value, textMapper.ParsingInfo, expressionName);
                case FunctionMapper function:
                    return BuildFunctionGetRuntimeHandler<T>(function, expressionName);
                case TupleMapper tupleMapper:
                    return BuildTupleGetRuntimeHandler<T>(tupleMapper, expressionName);
                case ReturnFunctionExpressionMapper returnFunction:
                    return BuildReturnFunctionDereferencementHandler<T>(returnFunction, expressionName);
                default:
                    throw new NotSupportedException($"Unknown GetRuntimeHandler : {expression}");
            }
        }

        private IGetRuntimeHandler<T> BuildReturnFunctionDereferencementHandler<T>(ReturnFunctionExpressionMapper returnFunction, string expressionName)
        {
            IGetRuntimeHandler<T> function = BuildGetRuntimeHandler<T>(returnFunction.Function);
            IGetterAccessor instanceVisitor;
            var outputType = _functionHandlerProvider.GetOutputType(returnFunction.Function.Identifier);
            if (options.Version == CompilerVersions.v2)
            {
                instanceVisitor = InstanceVisitorBuilder.GetGetterAccessor(outputType, returnFunction.Value.Children, sourcePropertyHandler, options);
            }
            else
            {
                instanceVisitor = new InstanceVisitor(outputType, returnFunction.Value.GetLitteral(), sourcePropertyHandler);
            }
            return new ReturnFunctionPropertyGetRuntimeHandler<T>(function, instanceVisitor, returnFunction.ParsingInfo, expressionName);
        }

        private IGetRuntimeHandler<T> BuildTupleGetRuntimeHandler<T>(TupleMapper tupleMapper, string expressionName)
        {
            var tupleParts = new List<IGetRuntimeHandler<T>>();
            foreach (var part in tupleMapper.Items)
            {
                tupleParts.Add(BuildGetRuntimeHandler<T>(part));
            }
            return new TupleGetRuntimeHandler<T>(tupleParts, tupleMapper.ParsingInfo, expressionName);
        }

        private IGetRuntimeHandler<T> BuildFunctionGetRuntimeHandler<T>(FunctionMapper functionMapper, string expressionName)
        {
            List<IGetRuntimeHandler<T>> arguments = new List<IGetRuntimeHandler<T>>();
            foreach (var arg in functionMapper.Arguments)
            {
                arguments.Add(BuildGetRuntimeHandler<T>(arg));
            }

            IExtractFunctionHandler<T> functionHandler = _functionHandlerProvider.Get<IExtractFunctionHandler<T>>(functionMapper.Identifier);
            if (functionHandler == null)
            {
                throw new MapperRuntimeException("Function identifier not found", functionMapper.ParsingInfo);
            }
            return new FunctionGetRuntimeHandler<T>(functionHandler, arguments, functionMapper.ParsingInfo, expressionName);
        }

        private IGetterAccessor BuildGetInstanceVisitor<T>(InstanceRefMapper instanceRef, IPropertyResolverHandler propertyResolver)
        {
            try
            {
                return InstanceVisitorBuilder.GetGetterAccessor<T>(instanceRef.Children, propertyResolver, options);
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException(exc.Message, instanceRef.ParsingInfo, exc);
            }
        }

        private IInstanceVisitor<T> BuildInstanceVisitor<T>(InstanceRefMapper instanceRef, IPropertyResolverHandler propertyResolver)
        {
            try
            {
                return new InstanceVisitor<T>(instanceRef.GetLitteral(), propertyResolver);
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException(exc.Message, instanceRef.ParsingInfo, exc);
            }
        }

        private ISetRuntimeHandler<T> BuildSetRuntimeHandler<T>(IExpressionMapper expression)
        {
            switch (expression)
            {
                case InsertInstanceRefMapper insertInstanceRef:
                    var instanceVisitor2 = options.Version == CompilerVersions.v2
                        ? InstanceVisitorBuilder.GetSetterAccessor<T>(insertInstanceRef.GetFieldInstanceRefs(), targetPropertyHandler, options)
                        : new InstanceVisitor<T>(insertInstanceRef.GetLitteral(), targetPropertyHandler);
                    return new SetRuntimeHandler<T>(instanceVisitor2, insertInstanceRef.ParsingInfo);
                case FunctionMapper function:
                    return BuildFunctionSetRuntimeHandler<T>(function);
            }
            return null;
        }

        private ISetRuntimeHandler<T> BuildFunctionSetRuntimeHandler<T>(FunctionMapper functionMapper)
        {
            List<IGetRuntimeHandler<T>> arguments = new List<IGetRuntimeHandler<T>>();
            foreach (var arg in functionMapper.Arguments)
            {
                arguments.Add(BuildGetRuntimeHandler<T>(arg));
            }

            IInsertFunctionHandler<T> insertFunctionHandler = _functionHandlerProvider.Get<IInsertFunctionHandler<T>>(functionMapper.Identifier);
            if (insertFunctionHandler == null)
            {
                throw new MapperRuntimeException("Function identifier not found", functionMapper.ParsingInfo);
            }
            return new FunctionSetRuntimeHandler<T>(insertFunctionHandler, arguments, functionMapper.ParsingInfo);
        }
    }
}
