namespace MapperDslLib.Parser
{
    public class InstanceRefMapper : IExpressionMapper
    {

        public InstanceRefMapper(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }
}