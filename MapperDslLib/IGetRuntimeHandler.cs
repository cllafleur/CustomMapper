namespace MapperDslLib
{
    internal interface IGetRuntimeHandler<T>
    {
        object Get(T obj);
    }
}