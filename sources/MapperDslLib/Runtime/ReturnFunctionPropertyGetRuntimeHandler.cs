using MapperDslLib.Parser;

namespace MapperDslLib.Runtime;

internal class ReturnFunctionPropertyGetRuntimeHandler<T> : IGetRuntimeHandler<T>
{
    private IGetRuntimeHandler<T> function;
    private InstanceVisitor instanceVisitor;
    private ParsingInfo parsingInfo;
    private string expressionName;

    public ReturnFunctionPropertyGetRuntimeHandler(IGetRuntimeHandler<T> function, InstanceVisitor instanceVisitor, ParsingInfo parsingInfo, string expressionName)
    {
        this.function = function;
        this.instanceVisitor = instanceVisitor;
        this.parsingInfo = parsingInfo;
        this.expressionName = expressionName;
    }

    public SourceResult Get(T obj)
    {
        try
        {
            var infos = instanceVisitor.GetLastPropertyInfo();
            var result = GetResult(obj);
            return new SourceResult()
            {
                Name = expressionName,
                Result = result,
                DataInfo = new DataSourceInfo() { PropertyInfo = infos }
            };
        }
        catch (Exception exc)
        {
            throw new MapperRuntimeException("Failed to get property", parsingInfo, exc);
        }

        IEnumerable<object> GetResult(T obj)
        {
            foreach (var funcResult in function.Get(obj).Result)
            {
                foreach (var result in instanceVisitor.GetInstance(funcResult))
                {
                    yield return result;
                }
            }
        }
    }
}
