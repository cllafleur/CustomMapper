using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    public interface IGetterAccessor
    {
        IEnumerable<object> GetInstance(object obj);
    }
}
