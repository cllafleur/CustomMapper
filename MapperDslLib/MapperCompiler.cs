using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib
{
    public class MapperCompiler<TOrigin, TTarget>
    {
        private IFunctionHandlerProvider _functionHandlerProvider;

        public MapperCompiler(IFunctionHandlerProvider functionHandlerProvider)
        {
            _functionHandlerProvider = functionHandlerProvider;
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
            switch (expression)
            {
                case InstanceRefMapper instanceRef:
                    var instanceVisitor = BuildInstanceVisitor<T>(instanceRef);
                    return new GetRuntimeHandler<T>(instanceVisitor, instanceRef.ParsingInfo);
                case TextMapper textMapper:
                    return new TextGetRuntimeHandler<T>(textMapper.Value, textMapper.ParsingInfo);
                case FunctionMapper function:
                    return BuildFunctionGetRuntimeHandler<T>(function);
            }
            return null;
        }

        private IGetRuntimeHandler<T> BuildFunctionGetRuntimeHandler<T>(FunctionMapper functionMapper)
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
            return new FunctionGetRuntimeHandler<T>(functionHandler, arguments, functionMapper.ParsingInfo);
        }

        private InstanceVisitor<T> BuildInstanceVisitor<T>(InstanceRefMapper instanceRef)
        {
            try
            {
                return new InstanceVisitor<T>(instanceRef.Value);
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
                case InstanceRefMapper instanceRef:
                    var instanceVisitor = BuildInstanceVisitor<T>(instanceRef);
                    return new SetRuntimeHandler<T>(instanceVisitor, instanceRef.ParsingInfo);
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
