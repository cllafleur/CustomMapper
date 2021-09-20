using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperDslLib.Runtime
{
    public interface IInsertTupleFunctionHandler<T>
    {
        void SetObject(T instanceObject, TupleSourceResult source, params object[] args);
    }
}
