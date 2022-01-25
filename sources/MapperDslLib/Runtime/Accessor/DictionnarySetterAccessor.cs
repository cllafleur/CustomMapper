using System.Collections;

namespace MapperDslLib.Runtime.Accessor;

class DictionnarySetterAccessor : ISetterAccessor
{
    private readonly IGetAccessor accessor;
    private readonly string key;

    public DictionnarySetterAccessor(IGetAccessor accessor, string key)
    {
        this.accessor = accessor;
        this.key = key;
    }

    public void SetInstance(object obj, IEnumerable<object> value)
    {
        var o = accessor != null ? accessor.GetInstance(obj).First() : obj;
        var v = value.FirstOrDefault();
        if (v is TupleValues values)
        {
            v = values.FirstOrDefault();
        }
        switch (o)
        {
            case IDictionary dic:
                dic[key] = v;
                break;
            case IDictionary<string, object> gdic:
                gdic[key] = v;
                break;
            case IDictionary<object, object> gdic2:
                gdic2[key] = v;
                break;
            default:
                var pis = o.GetType().GetProperties().Where(pi => pi.GetIndexParameters().Length > 0);
                pis.First(p => p.GetIndexParameters()[0].ParameterType.GUID == typeof(string).GUID).GetSetMethod().Invoke(o, new[] { key, v });
                break;
                //throw new NotSupportedException($"Not supported dictionary type {o.GetType()}");
        }
    }
}
