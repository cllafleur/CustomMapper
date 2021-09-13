using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    public class GetResult
    {
        public IEnumerable<object> Result { get; set; }

        internal bool InTuple { get; set; }
    }
}