namespace MapperDslLib
{
    public interface IInsertFunctionHandler
    {
        void SetObject(object instanceObject, object value, params object[] args);
    }
}