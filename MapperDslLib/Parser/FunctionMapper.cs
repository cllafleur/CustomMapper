namespace MapperDslLib.Parser
{
    using System.Collections.Generic;

    public class FunctionMapper : IExpressionMapper, INamedExpressionMapper
    {
        public string Identifier { get; private set; }

        public IEnumerable<IExpressionMapper> Arguments { get; private set; }

        public ParsingInfo ParsingInfo { get; }

        public string ExpressionName { get; set; }

        public FunctionMapper(string identifier, IEnumerable<IExpressionMapper> arguments, ParsingInfo infos)
        {
            Identifier = identifier;
            Arguments = arguments;
            ParsingInfo = infos;
        }
    }
}
