namespace MapperDslLib
{
    public class TextMapper : IExpressionMapper
    {
        public TextMapper(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}