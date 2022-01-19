using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class DictionarySetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, string identifier)
    {
        return new DictionnarySetterAccessor(getAccessor, identifier);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(Type type)
    {
        if (typeof(IDictionary).IsAssignableFrom(type)
                  || type.GetInterfaces().Where(t => t.GUID == typeof(IDictionary<,>).GUID).Any())
        {
            if (type.IsGenericType)
            {
                return (true, type.GenericTypeArguments[1]);
            }
            return (true, typeof(object));
        }
        return (false, null);
    }
}
