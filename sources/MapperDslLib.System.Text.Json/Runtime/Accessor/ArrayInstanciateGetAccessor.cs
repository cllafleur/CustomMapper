using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class ArrayInstanciateGetAccessor : IGetAccessor
{
    public IGetAccessor Next { get; set; }

    public IEnumerable<object> GetInstance(object obj)
    {
        return null;
    }
}
