using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    public class GetResult
    {
        public IEnumerable<object> Result { get; set; }
        public bool IsLiteral { get; internal set; }
        internal bool KeepEnumerate { get; set; }
    }
}