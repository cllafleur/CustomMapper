using System;
using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class TextGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private readonly string value;
        private readonly string expressionName;

        public TextGetRuntimeHandler(string value, Parser.ParsingInfo parsingInfo, string expressionName)
        {
            this.value = value;
            this.expressionName = expressionName;
        }

        public SourceResult Get(TOrigin obj)
        {
            var result = new SourceResult() { IsLiteral = true, Name = expressionName };
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