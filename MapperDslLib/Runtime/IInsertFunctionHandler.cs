using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    public interface IInsertFunctionHandler<T>
    {
        void SetObject(T instanceObject, IEnumerable<object> value, params object[] args);
    }
}