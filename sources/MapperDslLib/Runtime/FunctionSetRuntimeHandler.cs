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
            var values = new List<SourceResult>();
            var parameters = new Parameters();
            try
            {
                foreach (var arg in arguments)
                {
                    var argResult = arg.Get(obj);
                    values.Add(argResult);
                }
                parameters.Values = values.ToArray();
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                if (insertTupleFunctionHandler != null && value is TupleSourceResult tupleSource)
                {
                    insertTupleFunctionHandler.SetObject(obj, tupleSource, parameters);
                }
                else
                {
                    insertFunctionHandler.SetObject(obj, value, parameters);
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}