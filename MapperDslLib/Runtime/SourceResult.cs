using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    public class SourceResult
    {
        public IEnumerable<object> Result { get; set; }
        public bool IsLiteral { get; internal set; }
        internal bool KeepEnumerate { get; set; }
        public DataSourceInfo DataInfo { get; set; }
    }
}