using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class EnumerableGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    public (IGetAccessor getter, Type nextType) Create(string identifier, Type nextType)
    {
        return (new EnumeratorGetAccessor(), nextType);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(Type type)
    {
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
