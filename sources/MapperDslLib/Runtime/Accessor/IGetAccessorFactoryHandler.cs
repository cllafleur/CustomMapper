namespace MapperDslLib.Runtime.Accessor;

public interface IGetAccessorFactoryHandler
{
    (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos);
    (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type nextType);
}
