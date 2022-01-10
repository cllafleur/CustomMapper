using MapperDslLib.Parser;
using MapperDslLib.Runtime.Accessor;
using System;
using System.Collections.Generic;

namespace MapperDslLib.Runtime
{
    internal class SetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private IInstanceVisitor<T> instanceVisitor;
        private ParsingInfo parsingInfos;

        public SetRuntimeHandler(IInstanceVisitor<T> instanceVisitor, Parser.ParsingInfo parsingInfo)
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
            catch (Exception exc) when (exc is not MapperRuntimeException)
            {
                throw new MapperRuntimeException("Failed to set property", parsingInfos, exc);
            }
        }
    }
}