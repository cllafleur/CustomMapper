using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    public class SourceResult : SourceInfo
    {
        public IEnumerable<object> Result { get; set; }
    }
}