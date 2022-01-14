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
            switch (obj)
            {
                case IDictionary dic:
                    return Next.GetInstance(dic[key]);
                case IDictionary<string, object> gdic:
                    return Next.GetInstance(gdic[key]);
                case IDictionary<object, object> gdic2:
                    return Next.GetInstance(gdic2[key]);
                default:
                    throw new NotSupportedException($"Unsupported dictionary type {obj.GetType()}");
            }
        }
        else
        {
            switch (obj)
            {
                case IDictionary dic:
                    return new[] { dic[key] };
                case IDictionary<string, object> gdic:
                    return new[] { gdic[key] };
                case IDictionary<object, object> gdic2:
                    return new[] { gdic2[key] };
                default:
                    throw new NotSupportedException($"Unsupported dictionary type {obj.GetType()}");
            }
        }
    }
}
