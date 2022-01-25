using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ArraySetAccessor : ISetterAccessor
{
    private IGetAccessor getAccessor;
    private FieldInfos fieldInfos;

    public ArraySetAccessor(IGetAccessor getAccessor, FieldInfos fieldInfos)
    {
        this.getAccessor = getAccessor;
        this.fieldInfos = fieldInfos;
    }

    public void SetInstance(object obj, IEnumerable<object> value)
    {
        var o = getAccessor != null ? getAccessor.GetInstance(obj).First() : obj;
        JsonArray ja;
        if (string.IsNullOrEmpty(fieldInfos.Identifier))
        {
            ja = (JsonArray)o;
        }
        else
        {
            if (!((JsonObject)o).ContainsKey(fieldInfos.Identifier))
            {
                ((JsonObject)o).Add(fieldInfos.Identifier, new JsonArray());
            }
            ja = (JsonArray)((JsonObject)o)[fieldInfos.Identifier];
        }
        foreach (var v in value)
        {
            switch (v)
            {
                case string str:
                    ja.Add(JsonValue.Create(str));
                    break;
                case DateTime dt:
                    ja.Add(JsonValue.Create(dt));
                    break;
                case int i:
                    ja.Add(JsonValue.Create(i));
                    break;
                case long l:
                    ja.Add(JsonValue.Create(l));
                    break;
                case float f:
                    ja.Add(JsonValue.Create(f));
                    break;
                case double d:
                    ja.Add(JsonValue.Create(d));
                    break;
                case decimal de:
                    ja.Add(JsonValue.Create(de));
                    break;
                case bool b:
                    ja.Add(JsonValue.Create(b));
                    break;
                default:
                    ja.Add(JsonValue.Create(v));
                    break;
            }
        }
    }
}
