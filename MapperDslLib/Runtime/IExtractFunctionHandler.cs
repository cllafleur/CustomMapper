using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    public interface IExtractFunctionHandler<T>
    {
        SourceResult GetObject(T instanceObj, DataSourceInfo originInfo, IEnumerable<object>[] args);
    }
}