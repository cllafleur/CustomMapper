using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib
{
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
