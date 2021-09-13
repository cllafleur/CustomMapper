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

        public GetResult Get(TOrigin obj)
        {
            var result = new GetResult() { IsLiteral = true };
            result.Result = GetResults(result);
            return result;

            IEnumerable<object> GetResults(GetResult result)
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