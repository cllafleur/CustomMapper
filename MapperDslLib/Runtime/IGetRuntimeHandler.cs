using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal interface IGetRuntimeHandler<T>
    {
        SourceResult Get(T obj);
    }
}