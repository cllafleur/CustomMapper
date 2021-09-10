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

        public IEnumerable<object> Get(TOrigin obj)
        {
            List<object> values = new List<object>();
            try
            {
                foreach (var arg in arguments)
                {
                    values.AddRange(arg.Get(obj));
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