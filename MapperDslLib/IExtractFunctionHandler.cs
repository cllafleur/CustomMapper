namespace MapperDslLib
{
    public interface IExtractFunctionHandler
    {
        object GetObject(object instanceObj, params object[] args);
    }
}