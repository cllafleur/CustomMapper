using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class FieldSetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type currentType, IGetAccessor getAccessor, FieldInfos infos)
    {
        var prop = currentType.GetProperty(infos.Identifier);
        return new FieldSetterAccessor(getAccessor, prop);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
    {
        return (true, fieldInfos.OutputType);
    }
}
