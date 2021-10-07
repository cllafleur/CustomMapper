using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    public interface IInsertFunctionHandler<T>
    {
        void SetObject(T instanceObject, SourceResult source, Parameters parameters);
    }
}