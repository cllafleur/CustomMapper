using System;
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

        public SourceResult Get(TOrigin obj)
        {
            var result = new SourceResult() { IsLiteral = true };
            result.Result = GetResults(result);
            return result;

            IEnumerable<object> GetResults(SourceResult result)
            {
                var isFirst = true;
                while (result.KeepEnumerate || isFirst)
                {
                    isFirst = false;
                    yield return value;
                }
            }
        }
    }
}