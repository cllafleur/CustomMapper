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
            return new GetResult()
            {
                Result = GetScalar()
            };

            IEnumerable<object> GetScalar()
            {
                yield return value;
            }
        }
    }
}