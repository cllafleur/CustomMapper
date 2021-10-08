namespace MapperDslLib.Parser
{
    public class StatementMapper : IPartSourceProvider
    {

        public StatementMapper(IExpressionMapper originExpr, IExpressionMapper targetExpr, ParsingInfo infos)
        {
            this.OriginExpr = originExpr;
            this.TargetExpr = targetExpr;
            this.ParsingInfo = infos;
        }

        public IExpressionMapper OriginExpr { get; }
        public IExpressionMapper TargetExpr { get; }
        public ParsingInfo ParsingInfo { get; }
    }
}