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
            switch (obj)
            {
                case JsonObject jo:
                    return new[] { GetJsonObjectInstance(jo) };
                case JsonArray ja:
                    return HandleArray(ja);
            }
        }
        else
        {
            switch (obj)
            {
                case JsonObject jo:
                    return Next.GetInstance(GetJsonObjectInstance(jo));
                case JsonArray jo:
                    return HandleArray(jo);
            }
        }
        return Enumerable.Empty<object>();
    }

    private IEnumerable<object> HandleArray(JsonArray ja)
    {
        foreach (var o in ja)
        {
            if (Next != null)
            {
                foreach (var i in Next.GetInstance(GetJsonObjectInstance((JsonObject)o)))
                {
                    yield return i;
                }
            }
            else
            {
                yield return GetJsonObjectInstance((JsonObject)o);
            }
        }
        while (true)
        {
            var intermediate = new JsonObject();
            ja.Add(intermediate);
            var newObj = new JsonObject();
            intermediate.Add(identifier, newObj);
            if (Next != null)
            {
                foreach (var newO in Next.GetInstance(newObj))
                {
                    yield return newO;
                }
            }
            else
            {
                yield return newObj;
            }
        }
    }

    private object GetJsonObjectInstance(JsonObject obj)
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
