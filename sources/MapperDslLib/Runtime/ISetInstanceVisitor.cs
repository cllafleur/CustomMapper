namespace MapperDslLib.Runtime
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface ISetInstanceVisitor<T>
    {
        PropertyInfo GetLastPropertyInfo();
        void SetInstance(T obj, IEnumerable<object> result);
    }
}