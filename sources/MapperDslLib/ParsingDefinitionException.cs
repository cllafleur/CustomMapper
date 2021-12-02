using System.Runtime.Serialization;

namespace MapperDslLib
{
    [Serializable]
    internal class ParsingDefinitionException : Exception
    {
        public ParsingDefinitionException()
        {
        }

        public ParsingDefinitionException(string message) : base(message)
        {
        }

        public ParsingDefinitionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ParsingDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}