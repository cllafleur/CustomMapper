using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal interface IGetRuntimeHandler<T>
    {
        GetResult Get(T obj);
    }
}