using MapperDslLib.Parser;
using System;
using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class SetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private InstanceVisitor<T> instanceVisitor;
        private ParsingInfo parsingInfos;

        public SetRuntimeHandler(InstanceVisitor<T> instanceVisitor, Parser.ParsingInfo parsingInfo)
        {
            this.instanceVisitor = instanceVisitor;
            this.parsingInfos = parsingInfo;
        }

        public void SetValue(T obj, SourceResult value)
        {
            try
            {
                instanceVisitor.SetInstance(obj, value.Result);
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to set property", parsingInfos, exc);
            }
        }
    }
}