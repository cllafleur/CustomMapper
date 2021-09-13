using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    public interface IExtractFunctionHandler<T>
    {
        GetResult GetObject(T instanceObj, IEnumerable<object>[] args);
    }
}