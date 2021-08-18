namespace MapperDslLib.Runtime
{
    internal interface IStatementRuntimeHandler<TOrigin, TTarget>
    {
        void Evaluate(TOrigin origin, TTarget target);
    }
}