using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class GetRuntimeHandler<T> : IGetRuntimeHandler<T>
    {
        private readonly InstanceVisitor<T> instanceVisitor;
        private readonly ParsingInfo parsingInfos;
        private readonly string expressionName;

        public GetRuntimeHandler(InstanceVisitor<T> instanceVisitor, Parser.ParsingInfo parsingInfo, string expressionName)
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
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get property", parsingInfos, exc);
            }
        }
    }
}