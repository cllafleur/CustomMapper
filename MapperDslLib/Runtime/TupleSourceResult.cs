using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime
{
    public class TupleSourceResult : SourceResult
    {
        public new IEnumerable<TupleValues> Result
        {
            get { return (IEnumerable<TupleValues>)base.Result; }
            set { base.Result = value; }
        }
    }
}
