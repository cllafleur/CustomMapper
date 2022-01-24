namespace MapperDslLib.Runtime.Accessor;

public interface ISetAccessorFactoryHandler
{
    (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos);
    ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, FieldInfos fieldInfos);
}
