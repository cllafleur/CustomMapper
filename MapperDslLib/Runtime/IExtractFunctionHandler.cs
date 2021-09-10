using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    public interface IExtractFunctionHandler<T>
    {
        IEnumerable<object> GetObject(T instanceObj, params object[] args);
    }
}