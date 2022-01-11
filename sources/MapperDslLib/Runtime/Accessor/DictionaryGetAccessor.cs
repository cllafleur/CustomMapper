using System.Collections;

namespace MapperDslLib.Runtime.Accessor;

class DictionaryGetAccessor : IGetAccessor
{
    private string key;
    public IGetAccessor Next { get; set; }

    public DictionaryGetAccessor(string key)
    {
        this.key = key;
    }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj == null)
        {
            return Enumerable.Empty<object>();
        }
        if (Next != null)
        {
            return Next.GetInstance(((IDictionary)obj)[key]);
        }
        else
        {
            return new[] { ((IDictionary)obj)[key] };
        }
    }
}
