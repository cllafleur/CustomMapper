namespace MapperDslLib.Runtime.Accessor;

public interface IGetAccessorFactoryHandler
{
    (bool isTargetedType, Type nextType) DoesHandle(Type type);
    (IGetAccessor getter, Type nextType) Create(string identifier, Type nextType);
}
