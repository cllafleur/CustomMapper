using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ArrayInstanciateGetAccessor : IGetAccessor
{
    private FieldInfos fieldInfos;

    public ArrayInstanciateGetAccessor(FieldInfos fieldInfos)
    {
        this.fieldInfos = fieldInfos;
    }

    public IGetAccessor Next { get; set; }

    public IEnumerable<object> GetInstance(object obj)
    {
        foreach (var o in GetObjects(obj))
        {
            if (Next != null)
            {
                foreach (var i in Next.GetInstance(o))
                {
                    yield return i;
                }
            }
            else
            {
                yield return o;
            }
        }
    }

    private IEnumerable<object> GetObjects(object obj)
    {
        if (obj == null)
        {
            return Enumerable.Empty<object>();
        }
        var jo = obj as JsonObject;
        if (jo.ContainsKey(fieldInfos.Identifier))
        {
            return new[] { jo[fieldInfos.Identifier] };
        }
        var ja = new JsonArray();
        jo.Add(fieldInfos.Identifier, ja);
        return new[] { ja };
    }
}
