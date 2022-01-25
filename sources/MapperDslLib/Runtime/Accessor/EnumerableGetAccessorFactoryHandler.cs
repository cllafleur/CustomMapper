using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class EnumerableGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    public (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type nextType)
    {
        return (new EnumeratorGetAccessor(), nextType);
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
