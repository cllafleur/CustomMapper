using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    private bool instanciatePath;

    public ObjectGetAccessorFactoryHandler(bool instanciatePath = false)
    {
        this.instanciatePath = instanciatePath;
    }

    public (IGetAccessor getter, Type nextType) Create(string identifier, Type nextType)
    {
        if (instanciatePath)
        {
            return (new ObjectInstanciateGetAccessor(identifier), nextType);
        }
        return (new ObjectGetAccessor(identifier), nextType);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(Type type)
    {
        return (typeof(JsonNode).IsAssignableFrom(type), typeof(JsonNode));
    }
}
