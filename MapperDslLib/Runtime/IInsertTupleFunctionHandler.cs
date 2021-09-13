using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime
{
    public interface IInsertTupleFunctionHandler<T>
    {
        void SetObject(T instanceObject, IEnumerable<IEnumerable<object>> value, params object[] args);
    }
}
