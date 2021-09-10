using MapperDslLib.Parser;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    internal class GetRuntimeHandler<T> : IGetRuntimeHandler<T>
    {
        private InstanceVisitor<T> instanceVisitor;
        private ParsingInfo parsingInfos;

        public GetRuntimeHandler(InstanceVisitor<T> instanceVisitor, Parser.ParsingInfo parsingInfo)
        {
            this.instanceVisitor = instanceVisitor;
            this.parsingInfos = parsingInfo;
        }

        public IEnumerable<object> Get(T obj)
        {
            try
            {
                var instance = instanceVisitor.GetInstance(obj);
                return instance;
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to get property", parsingInfos, exc);
            }
        }
    }
}