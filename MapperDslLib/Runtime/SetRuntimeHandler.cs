using MapperDslLib.Parser;
using System;

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

        public void SetValue(T obj, object value)
        {
            try
            {
                instanceVisitor.SetInstance(obj, value);
            }
            catch (Exception exc)
            {
                throw new MapperRuntimeException("Failed to set property", parsingInfos, exc);
            }
        }
    }
}