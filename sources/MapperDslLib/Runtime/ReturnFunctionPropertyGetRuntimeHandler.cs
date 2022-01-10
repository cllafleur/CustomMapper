using MapperDslLib.Parser;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib.Runtime;

internal class ReturnFunctionPropertyGetRuntimeHandler<T> : IGetRuntimeHandler<T>
{
    private IGetRuntimeHandler<T> function;
    private IInstanceVisitor instanceVisitor;
    private ParsingInfo parsingInfo;
    private string expressionName;

    public ReturnFunctionPropertyGetRuntimeHandler(IGetRuntimeHandler<T> function, IInstanceVisitor instanceVisitor, ParsingInfo parsingInfo, string expressionName)
    {
        this.function = function;
        this.instanceVisitor = instanceVisitor;
        this.parsingInfo = parsingInfo;
        this.expressionName = expressionName;
    }

    public SourceResult Get(T obj)
    {
        var infos = instanceVisitor.GetLastPropertyInfo();
        var result = GetResult(obj);
        return new SourceResult()
        {
            Name = expressionName,
            Result = result,
            DataInfo = new DataSourceInfo() { PropertyInfo = infos }
        };

        IEnumerable<object> GetResult(T obj)
        {
            foreach (var funcResult in function.Get(obj).Result)
            {
                bool @continue = false;
                var enumerator = instanceVisitor.GetInstance(funcResult).GetEnumerator();

                try
                {
                    @continue = enumerator.MoveNext();
                }
                catch (Exception exc) when (exc is not MapperRuntimeException)
                {
                    throw new MapperRuntimeException("Failed to get property", parsingInfo, exc);
                }
                while (@continue)
                {
                    object result;
                    result = enumerator.Current;
                    yield return result;
                    try
                    {
                        @continue = enumerator.MoveNext();
                    }
                    catch (Exception exc) when (exc is not MapperRuntimeException)
                    {
                        throw new MapperRuntimeException("Failed to get property", parsingInfo, exc);
                    }
                }
            }
        }
    }
}
