using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectGetAccessor : IGetAccessor
{
    private string identifier;

    public ObjectGetAccessor(string identifier)
    {
        this.identifier = identifier;
    }

    public IGetAccessor Next { get; set; }

    public IEnumerable<object> GetInstance(object obj)
    {
        if (obj != null)
        {
            foreach (var o in GetResult(obj))
            {
                if (Next == null)
                {
                    JsonObject jo = (JsonObject)o;
                    yield return jo[identifier];
                }
                else
                {
                    JsonObject jo = (JsonObject)o;
                    var value  = jo[identifier];
                    foreach (var i in Next.GetInstance(value))
                    {
                        yield return i;
                    }
                }
            }
        }
    }

    private IEnumerable<object> GetResult(object obj)
    {
        switch (obj)
        {
            case JsonObject jo:
                yield return jo;
                break;
            case JsonArray ja:
                foreach (var o in ja)
                {
                    foreach (var i in GetResult(o))
                    {
                        yield return i;
                    }
                }
                break;
        }
    }
}
