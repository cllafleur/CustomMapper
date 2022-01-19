namespace MapperDslLib.Runtime.Accessor;

public interface ISetAccessorFactoryHandler
{
    (bool isTargetedType, Type nextType) DoesHandle(Type type);
    ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, string identifier);
}