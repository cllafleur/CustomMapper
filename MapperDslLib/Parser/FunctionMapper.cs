namespace MapperDslLib.Parser
{
    using System.Collections.Generic;

    public class FunctionMapper : IExpressionMapper
    {
        public string Identifier { get; private set; }

        public IEnumerable<IExpressionMapper> Arguments { get; private set; }

        public FunctionMapper(string identifier, IEnumerable<IExpressionMapper> arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }
    }
}
