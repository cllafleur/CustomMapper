using System.Collections;

namespace MapperDslLib.Runtime.Accessor;

internal class DictionaryGetAccessorFactoryHandler : IGetAccessorFactoryHandler
{
    public (IGetAccessor getter, Type nextType) Create(string identifier, Type nextType)
    {
        return (new DictionaryGetAccessor(identifier), nextType);
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
