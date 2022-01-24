using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    private const string ArrayIdentifier = "*";
    private bool instanciatePath;

    public ObjectGetAccessorFactoryHandler(bool instanciatePath = false)
    {
        this.instanciatePath = instanciatePath;
    }

    public (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type nextType)
    {
        var identifier = fieldInfos.Identifier;
        if (instanciatePath)
        {
            if (identifier == ArrayIdentifier)
            {
                return (new ArrayInstanciateGetAccessor(), nextType);
            }
            return (new ObjectInstanciateGetAccessor(identifier), nextType);
        }
        return (new ObjectGetAccessor(identifier), nextType);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
    {
        return (typeof(JsonNode).IsAssignableFrom(fieldInfos.OutputType), typeof(JsonNode));
    }
}
