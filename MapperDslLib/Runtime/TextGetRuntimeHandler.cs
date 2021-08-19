namespace MapperDslLib.Runtime
{
    internal class TextGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private string value;

        public TextGetRuntimeHandler(string value, Parser.ParsingInfo parsingInfo)
        {
            this.value = value;
        }

        public object Get(TOrigin obj)
        {
            return value;
        }
    }
}