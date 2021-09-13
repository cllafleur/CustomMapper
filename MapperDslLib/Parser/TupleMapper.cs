using System.Collections.Generic;

namespace MapperDslLib.Parser
{
    public class TupleMapper : IExpressionMapper
    {
        private IExpressionMapper[] items;
        private ParsingInfo parsingInfo;

        public IExpressionMapper[] Items => items;

        public TupleMapper(IExpressionMapper[] items, ParsingInfo parsingInfo)
        {
            this.items = items;
            this.parsingInfo = parsingInfo;
        }

        public ParsingInfo ParsingInfo => parsingInfo;
    }
}