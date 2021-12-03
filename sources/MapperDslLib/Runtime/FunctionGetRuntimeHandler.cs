using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class FunctionGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private readonly IExtractFunctionHandler<TOrigin> functionHandler;
        private readonly IEnumerable<IGetRuntimeHandler<TOrigin>> arguments;
        private readonly ParsingInfo parsingInfos;
        private readonly string expressionName;

        public FunctionGetRuntimeHandler(IExtractFunctionHandler<TOrigin> functionHandler, IEnumerable<IGetRuntimeHandler<TOrigin>> arguments, Parser.ParsingInfo parsingInfo, string expressionName)
        {
            this.functionHandler = functionHandler;
            this.arguments = arguments;
            this.parsingInfos = parsingInfo;
            this.expressionName = expressionName;
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
            catch (Exception exc) when (exc is not MapperRuntimeException)
            {
                throw new MapperRuntimeException("Failed to get parameters", parsingInfos, exc);
            }
            try
            {
                var result = functionHandler.GetObject(obj, parameters);
                result.Name = expressionName;
                return result;
            }
            catch (Exception exc) when (exc is not MapperRuntimeException)
            {
                throw new MapperRuntimeException("Failed to call function", parsingInfos, exc);
            }
        }
    }
}