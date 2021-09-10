using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal interface IGetRuntimeHandler<T>
    {
        IEnumerable<object> Get(T obj);
    }
}