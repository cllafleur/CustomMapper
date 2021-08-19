using MapperDslLib.Parser;
using System;
using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class FunctionSetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private IInsertFunctionHandler<T> insertFunctionHandler;
        private List<IGetRuntimeHandler<T>> arguments;
        private ParsingInfo parsingInfos;

        public FunctionSetRuntimeHandler(IInsertFunctionHandler<T> insertFunctionHandler, List<IGetRuntimeHandler<T>> arguments, Parser.ParsingInfo parsingInfo)
        {
            this.insertFunctionHandler = insertFunctionHandler;
            this.arguments = arguments;
            this.parsingInfos = parsingInfo;
        }

        public void SetValue(T obj, object value)
        {
            List<object> parameters = new List<object>();
            try
            {
                foreach (var arg in arguments)
                {
                    parameters.Add(arg.Get(obj));
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                insertFunctionHandler.SetObject(obj, value, parameters.ToArray());
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}