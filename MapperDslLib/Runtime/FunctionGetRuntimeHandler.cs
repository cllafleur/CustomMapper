using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class FunctionGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private IExtractFunctionHandler<TOrigin> functionHandler;
        private IEnumerable<IGetRuntimeHandler<TOrigin>> arguments;
        private ParsingInfo parsingInfos;

        public FunctionGetRuntimeHandler(IExtractFunctionHandler<TOrigin> functionHandler, IEnumerable<IGetRuntimeHandler<TOrigin>> arguments, Parser.ParsingInfo parsingInfo)
        {
            this.functionHandler = functionHandler;
            this.arguments = arguments;
            this.parsingInfos = parsingInfo;
        }

        public SourceResult Get(TOrigin obj)
        {
            var values = new List<SourceResult>();
            var parameters = new Parameters();
            try
            {
                foreach (var arg in arguments)
                {
                    var result = arg.Get(obj);
                    values.Add(result);
                }
                parameters.Values = values.ToArray();
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                var result = functionHandler.GetObject(obj, parameters);
                return result;
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}