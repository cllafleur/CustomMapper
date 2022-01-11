using System.Reflection;

namespace MapperDslLib.Runtime.Accessor;

class GetterAccessor : IGetterAccessor
{
    private readonly IGetAccessor accessor;

    public GetterAccessor(IGetAccessor accessor)
    {
        this.accessor = accessor;
    }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj == null)
        {
            return Enumerable.Empty<object>();
        }
        return accessor.GetInstance(obj);
    }

    public PropertyInfo GetPropertyInfo()
    {
        return null;
    }
}
