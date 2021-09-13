using MapperDslLib.Parser;
using System;
using System.Collections.Generic;

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

        public GetResult Get(TOrigin obj)
        {
            var values = new List<IEnumerable<object>>();
            try
            {
                foreach (var arg in arguments)
                {
                    values.Add(arg.Get(obj).Result);
                }
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                var result = functionHandler.GetObject(obj, values.ToArray());
                return result;
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}