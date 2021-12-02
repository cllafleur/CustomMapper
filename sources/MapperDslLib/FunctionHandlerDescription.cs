namespace MapperDslLib
{
    internal class FunctionHandlerDescription
    {
        public FunctionHandlerDescription(Type functionHandlerType, Type outputType)
        {
            FunctionHandlerType = functionHandlerType;
            OutputType = outputType;
        }

        public Type FunctionHandlerType { get; }
        public Type OutputType { get; }
    }
}