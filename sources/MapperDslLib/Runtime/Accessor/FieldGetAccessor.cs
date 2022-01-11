using System.Reflection;

namespace MapperDslLib.Runtime.Accessor;

class FieldGetAccessor : IGetAccessor
{
    private PropertyInfo prop;

    public IGetAccessor Next { get; set; }

    public FieldGetAccessor(PropertyInfo prop)
    {
        this.prop = prop;
    }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj != null)
        {
            if (Next == null)
            {
                return new object[] { prop.GetValue(obj) };
            }
            else
            {
                return Next.GetInstance(prop.GetValue(obj));
            }
        }
        return Enumerable.Empty<object>();
    }
}
