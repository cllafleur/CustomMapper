using MapperDslLib.Parser;
using System;
using System.Collections.Generic;

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

        public void SetValue(T obj, GetResult value)
        {
            List<object> parameters = new List<object>();
            try
            {
                foreach (var arg in arguments)
                {
                    parameters.AddRange(arg.Get(obj).Result);
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
                    insertTupleFunctionHandler.SetObject(obj, tupleEnumerable, parameters.ToArray());
                }
                else
                {
                    insertFunctionHandler.SetObject(obj, value.Result, parameters.ToArray());
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}