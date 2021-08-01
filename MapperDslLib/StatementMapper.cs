namespace MapperDslLib
{
    public class StatementMapper
    {

        public StatementMapper(IExpressionMapper originExpr, IExpressionMapper targetExpr)
        {
            this.OriginExpr = originExpr;
            this.TargetExpr = targetExpr;
        }

        public IExpressionMapper OriginExpr { get; private set; }
        public IExpressionMapper TargetExpr { get; private set; }
    }
}