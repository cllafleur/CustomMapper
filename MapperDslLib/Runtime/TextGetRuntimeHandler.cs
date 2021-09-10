using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class TextGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private string value;

        public TextGetRuntimeHandler(string value, Parser.ParsingInfo parsingInfo)
        {
            this.value = value;
        }

        public IEnumerable<object> Get(TOrigin obj)
        {
            yield return value;
        }
    }
}