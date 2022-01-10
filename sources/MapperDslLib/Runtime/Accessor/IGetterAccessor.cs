using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    public interface IGetterAccessor
    {
        IEnumerable<object> GetInstance(object obj);
        PropertyInfo GetPropertyInfo();
        void AddNext(IGetterAccessor getter);
    }
}
