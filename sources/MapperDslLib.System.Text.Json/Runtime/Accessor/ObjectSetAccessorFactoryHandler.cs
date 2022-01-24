using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class ObjectSetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, FieldInfos infos)
    {
        return new ObjectSetAccessor(getAccessor, infos.Identifier);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
    {
        return (true, null);
    }
}
