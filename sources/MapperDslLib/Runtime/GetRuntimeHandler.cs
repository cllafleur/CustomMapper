using MapperDslLib.Parser;
using MapperDslLib.Runtime.Accessor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class GetRuntimeHandler<T> : IGetRuntimeHandler<T>
    {
        private readonly IInstanceVisitor<T> instanceVisitor;
        private readonly ParsingInfo parsingInfos;
        private readonly string expressionName;

        public GetRuntimeHandler(IInstanceVisitor<T> instanceVisitor, Parser.ParsingInfo parsingInfo, string expressionName)
        {
            this.instanceVisitor = instanceVisitor;
            this.parsingInfos = parsingInfo;
            this.expressionName = expressionName;
        }

        public SourceResult Get(T obj)
        {
            try
            {
                var instance = instanceVisitor.GetInstance(obj);
                var infos = instanceVisitor.GetLastPropertyInfo();
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