using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class EnumerableSetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, FieldInfos infos)
    {
        return new EnumeratorSetterAccessor(getAccessor);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
    {
        var type = fieldInfos.OutputType;
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            Type nextType = null;
            if (type.IsGenericType)
            {
                nextType = type.GenericTypeArguments[0];
            }
            else if (type.IsArray)
            {
                nextType = type.GetElementType();
            }
            return (true, nextType);
        }
        return (false, null);
    }
}
