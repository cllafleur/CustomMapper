namespace MapperDslLib
{
    internal interface IStatementRuntimeHandler<TOrigin, TTarget>
    {
        void Evaluate(TOrigin origin, TTarget target);
    }
}