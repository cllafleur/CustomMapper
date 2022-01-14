using System.Collections;

namespace MapperDslLib.Runtime.Accessor;
class EnumeratorSetterAccessor : ISetterAccessor
{
    private readonly IGetAccessor accessor;

    public EnumeratorSetterAccessor(IGetAccessor accessor)
    {
        this.accessor = accessor;
    }

    public void SetInstance(object obj, IEnumerable<object> value)
    {
        var o = accessor != null ? accessor.GetInstance(obj).First() : obj;
        foreach (var v in value)
        {
            ((IList)o).Add(v);
        }
    }
}
