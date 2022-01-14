using System.Globalization;
using System.Reflection;

namespace MapperDslLib.Runtime.Accessor;

class FieldSetterAccessor : ISetterAccessor
{
    private IGetAccessor accessor;
    private PropertyInfo prop;

    public FieldSetterAccessor(IGetAccessor accessor, PropertyInfo prop)
    {
        this.accessor = accessor;
        this.prop = prop;
    }

    public void SetInstance(object obj, IEnumerable<object> value)
    {
        var o = accessor != null ? accessor.GetInstance(obj).First() : obj;
        var v = value.FirstOrDefault();
        if (v is TupleValues values)
        {
            v = values.FirstOrDefault();
        }
        object convertedValue = null;
        Type innerType = Nullable.GetUnderlyingType(prop.PropertyType);
        if (innerType != null && innerType == v.GetType())
        {
            convertedValue = v;
        }
        else
        {
            convertedValue = value == null ? null : Convert.ChangeType(v, prop.PropertyType, CultureInfo.InvariantCulture);
        }
        prop.SetValue(o, convertedValue);
    }
}
