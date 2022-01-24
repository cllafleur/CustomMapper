using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectInstanciateGetAccessor : IGetAccessor
{
    private string identifier;

    public ObjectInstanciateGetAccessor(string identifier)
    {
        this.identifier = identifier;
    }

    public IGetAccessor Next { get; set; }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj == null)
        {
            return Enumerable.Empty<object>();
        }
        if (Next == null)
        {
            JsonObject jo = (JsonObject)obj;
            return new[] { GetInstance(jo) };
        }
        else
        {
            JsonObject jo = (JsonObject)obj;
            return Next.GetInstance(GetInstance(jo));
        }
    }

    private object GetInstance(JsonObject obj)
    {
        if (obj.ContainsKey(identifier))
        {
            return obj[identifier];
        }
        JsonObject newObj = new JsonObject();
        obj.Add(identifier, newObj);
        return newObj;
    }
}
