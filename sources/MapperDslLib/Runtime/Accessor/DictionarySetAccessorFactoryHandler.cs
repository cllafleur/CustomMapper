using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor;

internal class DictionarySetAccessorFactoryHandler : ISetAccessorFactoryHandler
{
    public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, FieldInfos infos)
    {
        return new DictionnarySetterAccessor(getAccessor, infos.Identifier);
    }

    public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
    {
        var type = fieldInfos.OutputType;
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
