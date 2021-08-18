namespace MapperDslLib.Runtime
{
    internal interface IGetRuntimeHandler<T>
    {
        object Get(T obj);
    }
}