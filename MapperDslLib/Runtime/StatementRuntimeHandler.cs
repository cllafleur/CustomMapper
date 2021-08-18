namespace MapperDslLib.Runtime
{
    internal class StatementRuntimeHandler<TOrigin, TTarget> : IStatementRuntimeHandler<TOrigin, TTarget>
    {
        private IGetRuntimeHandler<TOrigin> getRuntimeHandler;
        private ISetRuntimeHandler<TTarget> setRuntimeHandler;

        public StatementRuntimeHandler(IGetRuntimeHandler<TOrigin> getRuntimeHandler, ISetRuntimeHandler<TTarget> setRuntimeHandler)
        {
            this.getRuntimeHandler = getRuntimeHandler;
            this.setRuntimeHandler = setRuntimeHandler;
        }

        public void Evaluate(TOrigin origin, TTarget target)
        {
            var value = getRuntimeHandler.Get(origin);
            setRuntimeHandler.SetValue(target, value);
        }
    }
}