namespace MapperDslLib.Parser
{
    public class FieldInstanceRefMapper : IExpressionMapper
    {

        public FieldInstanceRefMapper(string value, ParsingInfo infos)
        {
            Value = value;
            ParsingInfo = infos;
        }

        public string Value { get; private set; }

        public ParsingInfo ParsingInfo { get; }
    }
}