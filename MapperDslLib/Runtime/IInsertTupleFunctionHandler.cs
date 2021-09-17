using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperDslLib.Runtime
{
    public interface IInsertTupleFunctionHandler<T>
    {
        void SetObject(T instanceObject, DataSourceInfo originInfos, IEnumerable<IEnumerable<object>> value, params object[] args);
    }
}
