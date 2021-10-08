using MapperDslLib.Parser;
using System;

namespace MapperDslLib.Runtime
{
    internal class StatementRuntimeHandler<TOrigin, TTarget> : IStatementRuntimeHandler<TOrigin, TTarget>
    {
        private IGetRuntimeHandler<TOrigin> getRuntimeHandler;
        private ISetRuntimeHandler<TTarget> setRuntimeHandler;
        private ParsingInfo parsingInfos;

        public StatementRuntimeHandler(IGetRuntimeHandler<TOrigin> getRuntimeHandler, ISetRuntimeHandler<TTarget> setRuntimeHandler, ParsingInfo infos)
        {
            this.getRuntimeHandler = getRuntimeHandler;
            this.setRuntimeHandler = setRuntimeHandler;
            this.parsingInfos = infos;
        }

        public void Evaluate(TOrigin origin, TTarget target)
        {
            try
            {
                var value = getRuntimeHandler.Get(origin);
                setRuntimeHandler.SetValue(target, value);
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException($"Failed to evaluate Statement", parsingInfos, exc);
            }
        }
    }
}