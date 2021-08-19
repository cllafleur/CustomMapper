namespace MapperDslLib.Parser
{
    public class TextMapper : IExpressionMapper
    {
        public TextMapper(string value, ParsingInfo infos)
        {
            Value = value;
            ParsingInfo = infos;
        }

        public string Value { get; }

        public ParsingInfo ParsingInfo { get; }
    }
}