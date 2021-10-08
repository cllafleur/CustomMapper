namespace MapperDslLib.Parser
{
    public class InstanceRefMapper : IExpressionMapper, INamedExpressionMapper
    {

        public InstanceRefMapper(string value, ParsingInfo infos)
        {
            Value = value;
            ParsingInfo = infos;
        }

        public string Value { get; private set; }

        public ParsingInfo ParsingInfo { get; }

        public string ExpressionName { get; set; }
    }
}