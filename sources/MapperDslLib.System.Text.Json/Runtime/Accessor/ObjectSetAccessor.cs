using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectSetAccessor : ISetterAccessor
{
    private IGetAccessor getAccessor;
    private string identifier;

    public ObjectSetAccessor(IGetAccessor getAccessor, string identifier)
    {
        this.getAccessor = getAccessor;
        this.identifier = identifier;
    }

    public void SetInstance(object obj, IEnumerable<object> value)
    {
        var eo = getAccessor != null ? getAccessor.GetInstance(obj) : new[] { obj };
        var enumerator = eo.GetEnumerator();
        foreach (var v in value)
        {
            if (!enumerator.MoveNext())
            {
                break;
            }
            var o = enumerator.Current;

            switch (o)
            {
                case JsonObject jo:
                    SetObject(v, jo);
                    break;
                case JsonArray ja:
                    foreach (var va in value)
                    {
                        SetArray(va, ja);
                    }
                    break;
            }
        }
    }

    private void SetArray(object value, JsonArray ja)
    {
        var jo = SetObject(value, null);
        ja.Add(jo);
    }

    private JsonObject SetObject(object value, JsonObject jo)
    {
        jo ??= new JsonObject();
        switch (value)
        {
            case string str:
                jo.Add(identifier, JsonValue.Create(str));
                break;
            case DateTime dt:
                jo.Add(identifier, JsonValue.Create(dt));
                break;
            case int i:
                jo.Add(identifier, JsonValue.Create(i));
                break;
            case long l:
                jo.Add(identifier, JsonValue.Create(l));
                break;
            case float f:
                jo.Add(identifier, JsonValue.Create(f));
                break;
            case double d:
                jo.Add(identifier, JsonValue.Create(d));
                break;
            case decimal de:
                jo.Add(identifier, JsonValue.Create(de));
                break;
            case bool b:
                jo.Add(identifier, JsonValue.Create(b));
                break;
            default:
                jo.Add(identifier, JsonValue.Create(value));
                break;
        }
        return jo;
    }
}
