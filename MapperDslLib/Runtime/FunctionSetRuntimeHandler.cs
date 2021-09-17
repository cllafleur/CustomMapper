using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class FunctionSetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private IInsertFunctionHandler<T> insertFunctionHandler;
        private IInsertTupleFunctionHandler<T> insertTupleFunctionHandler;
        private List<IGetRuntimeHandler<T>> arguments;
        private ParsingInfo parsingInfos;

        public FunctionSetRuntimeHandler(IInsertFunctionHandler<T> insertFunctionHandler, List<IGetRuntimeHandler<T>> arguments, Parser.ParsingInfo parsingInfo)
        {
            this.insertFunctionHandler = insertFunctionHandler;
            this.insertTupleFunctionHandler = insertFunctionHandler as IInsertTupleFunctionHandler<T>;
            this.arguments = arguments;
            this.parsingInfos = parsingInfo;
        }

        public void SetValue(T obj, SourceResult value)
        {
            List<object> parameters = new List<object>();
            try
            {
                foreach (var arg in arguments)
                {
                    var argResult = arg.Get(obj);
                    parameters.AddRange(argResult.Result);
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                if (insertTupleFunctionHandler != null && value.Result is IEnumerable<TupleValues> tupleEnumerable)
                {
                    insertTupleFunctionHandler.SetObject(obj, null, tupleEnumerable, parameters.ToArray());
                }
                else
                {
                    insertFunctionHandler.SetObject(obj, value, parameters.ToArray());
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}