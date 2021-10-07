using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime
{
    public class SourceInfo
    {
        public bool IsLiteral { get; internal set; }
        public DataSourceInfo DataInfo { get; set; }
    }
}
