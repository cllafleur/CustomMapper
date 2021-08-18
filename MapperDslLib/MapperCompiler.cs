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
            return new StatementRuntimeHandler<TOrigin, TTarget>(getRuntimeHandler, setRuntimeHandler);
        }

        private IGetRuntimeHandler<T> BuildGetRuntimeHandler<T>(IExpressionMapper expression)
        {
            switch (expression)
            {
                case InstanceRefMapper instanceRef:
                    var instanceVisitor = BuildInstanceVisitor<T>(instanceRef);
                    return new GetRuntimeHandler<T>(instanceVisitor);
                case TextMapper textMapper:
                    return new TextGetRuntimeHandler<T>(textMapper.Value);
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
            return new FunctionGetRuntimeHandler<T>(_functionHandlerProvider.Get<IExtractFunctionHandler<T>>(functionMapper.Identifier), arguments);
        }

        private InstanceVisitor<T> BuildInstanceVisitor<T>(InstanceRefMapper instanceRef)
        {
            return new InstanceVisitor<T>(instanceRef.Value);
        }

        private ISetRuntimeHandler<T> BuildSetRuntimeHandler<T>(IExpressionMapper expression)
        {
            switch (expression)
            {
                case InstanceRefMapper instanceRef:
                    var instanceVisitor = BuildInstanceVisitor<T>(instanceRef);
                    return new SetRuntimeHandler<T>(instanceVisitor);
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
            return new FunctionSetRuntimeHandler<T>(_functionHandlerProvider.Get<IInsertFunctionHandler<T>>(functionMapper.Identifier), arguments);
        }
    }
}
