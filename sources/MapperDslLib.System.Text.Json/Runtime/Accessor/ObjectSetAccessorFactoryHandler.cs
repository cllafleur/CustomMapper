using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectSetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, string identifier)
    {
        return new ObjectSetAccessor(getAccessor, identifier);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(Type type)
    {
        return (true, null);
    }
}
