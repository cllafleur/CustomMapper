namespace MapperDslLib.Runtime
{
    public interface IInsertFunctionHandler<T>
    {
        void SetObject(T instanceObject, object value, params object[] args);
    }
}