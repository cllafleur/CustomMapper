using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class DeconstructJsonNodeAccessor : IGetAccessor
{
    public IGetAccessor Next { get => throw new NotSupportedException("DeconstructJsonNodeAccessor"); set => throw new NotSupportedException("DeconstructJsonNodeAccessor"); }

    public IEnumerable<object> GetInstance(object obj)
    {
        switch (obj)
        {
            case JsonValue jv:
            yield return GetValue(jv);
                break;
            case JsonArray ja:
                foreach (var v in ja)
                {
                    yield return GetValue(v.AsValue());
                }
                break;
        }

        object GetValue(JsonValue o)
        {
            if (o.TryGetValue(out string str)) return str;
            if (o.TryGetValue(out DateTime dt)) return dt;
            if (o.TryGetValue(out int i)) return i;
            if (o.TryGetValue(out long l)) return l;
            if (o.TryGetValue(out float f)) return f;
            if (o.TryGetValue(out double d)) return d;
            if (o.TryGetValue(out decimal de)) return de;
            if (o.TryGetValue(out bool b)) return b;
            return o.ToString();
        }
    }
}
