namespace MapperDslLib.Parser
{
    using System.Collections.Generic;

    public class FunctionMapper : IExpressionMapper
    {
        public string Identifier { get; private set; }

        public IEnumerable<IExpressionMapper> Arguments { get; private set; }

        public ParsingInfo ParsingInfo { get; }

        public FunctionMapper(string identifier, IEnumerable<IExpressionMapper> arguments, ParsingInfo infos)
        {
            Identifier = identifier;
            Arguments = arguments;
            ParsingInfo = infos;
        }
    }
}
