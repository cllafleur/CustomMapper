using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    public interface ISetterAccessor
    {
        void SetInstance(object obj, IEnumerable<object> value);
    }
}
