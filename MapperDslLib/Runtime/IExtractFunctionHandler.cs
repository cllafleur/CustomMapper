namespace MapperDslLib.Runtime
{
    public interface IExtractFunctionHandler<T>
    {
        object GetObject(T instanceObj, params object[] args);
    }
}