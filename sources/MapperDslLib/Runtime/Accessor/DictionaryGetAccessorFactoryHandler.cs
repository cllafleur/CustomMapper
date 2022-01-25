using System.Collections;

namespace MapperDslLib.Runtime.Accessor;

internal class DictionaryGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    public (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type nextType)
    {
        return (new DictionaryGetAccessor(fieldInfos.Identifier), nextType);
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
