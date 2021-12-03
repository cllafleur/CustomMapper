using MapperDslLib.Parser;
using System;
using System.Runtime.Serialization;

namespace MapperDslLib.Runtime
{
    [Serializable]
    public class MapperRuntimeException : Exception
    {
        private ParsingInfo parsingInfos;

        public ParsingInfo ParsingInfo => parsingInfos;

        public MapperRuntimeException()
        {
        }

        public MapperRuntimeException(string message) : base(message)
        {
        }

        public MapperRuntimeException(string message, ParsingInfo parsingInfos) : base(message)
        {
            this.parsingInfos = parsingInfos;
        }

        public MapperRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MapperRuntimeException(string message, ParsingInfo parsingInfos, Exception innerException) : this($"{message}\nLine: {parsingInfos.Line} Expression: '{parsingInfos.Text}'", innerException)
        {
            this.parsingInfos = parsingInfos;
        }

        protected MapperRuntimeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}