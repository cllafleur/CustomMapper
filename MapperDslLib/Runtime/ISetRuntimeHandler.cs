namespace MapperDslLib.Runtime
{
    internal interface ISetRuntimeHandler<T>
    {
        void SetValue(T obj, object value);
    }
}