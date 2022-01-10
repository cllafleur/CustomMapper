using MapperDslLib.Parser;
using MapperDslLib.Runtime.Accessor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class GetRuntimeHandler<T> : IGetRuntimeHandler<T>
    {
        private readonly IGetterAccessor accessor;
        private readonly ParsingInfo parsingInfos;
        private readonly string expressionName;

        public GetRuntimeHandler(IGetterAccessor accessor, Parser.ParsingInfo parsingInfo, string expressionName)
        {
            this.accessor = accessor;
            this.parsingInfos = parsingInfo;
            this.expressionName = expressionName;
        }

        public SourceResult Get(T obj)
        {
            try
            {
                var instance = accessor.GetInstance(obj);
                var infos = accessor.GetPropertyInfo();
                return new SourceResult()
                {
                    Name = expressionName,
                    Result = instance,
                    DataInfo = new DataSourceInfo() { PropertyInfo = infos }
                };
            }
            catch (Exception exc) when (exc is not MapperRuntimeException)
            {
                throw new MapperRuntimeException("Failed to get property", parsingInfos, exc);
            }
        }
    }
}