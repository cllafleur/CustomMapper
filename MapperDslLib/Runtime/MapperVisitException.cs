using System;
using System.Runtime.Serialization;

namespace MapperDslLib.Runtime
{
    [Serializable]
    public class MapperVisitException : Exception
    {
        public MapperVisitException()
        {
        }

        public MapperVisitException(string message) : base(message)
        {
        }

        public MapperVisitException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MapperVisitException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}