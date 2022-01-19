using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class FieldSetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type currentType, IGetAccessor getAccessor, string identifier)
    {
        var prop = currentType.GetProperty(identifier);
        return new FieldSetterAccessor(getAccessor, prop);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(Type type)
    {
        return (true, type);
    }
}
