using System.Reflection;

namespace MapperDslLib.Runtime.Accessor;

class GetterAccessor : IGetterAccessor
{
    private readonly IGetAccessor accessor;
    private readonly IGetAccessor deconstructorAccessor;

    public GetterAccessor(IGetAccessor accessor, IGetAccessor deconstructorAccessor)
    {
        this.accessor = accessor;
        this.deconstructorAccessor = deconstructorAccessor;
    }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj != null)
        {
            foreach (var o in accessor.GetInstance(obj))
            {
                if (deconstructorAccessor != null)
                {
                    foreach (var v in deconstructorAccessor.GetInstance(o))
                    {
                        yield return v;
                    }
                }
                else
                {
                    yield return o;
                }
            }
        }
    }

    public PropertyInfo GetPropertyInfo()
    {
        return null;
    }
}
