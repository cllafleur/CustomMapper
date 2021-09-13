using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal interface ISetRuntimeHandler<T>
    {
        void SetValue(T obj, GetResult value);
    }
}