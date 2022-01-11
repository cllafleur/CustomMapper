using System.Collections;

namespace MapperDslLib.Runtime.Accessor;

class EnumeratorGetAccessor : IGetAccessor
{
    public IGetAccessor Next { get; set; }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj == null)
        {
            return Enumerable.Empty<object>();
        }
        if (Next == null)
        {
            return (IEnumerable<object>)obj;
        }
        return CreateResult(obj);

        IEnumerable<object> CreateResult(object obj)
        {
            foreach (var i in (IEnumerable)obj)
            {
                foreach (var o in Next.GetInstance(i))
                {
                    yield return o;
                }
            }
        }
    }
}
