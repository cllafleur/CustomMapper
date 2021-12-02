namespace MapperDslLib.Runtime
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IGetInstanceVisitor<T>
    {
        IEnumerable<object> GetInstance(T obj);
        PropertyInfo GetLastPropertyInfo();
    }
}