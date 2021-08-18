namespace MapperDslLib.Runtime
{
    internal class TextGetRuntimeHandler<TOrigin> : IGetRuntimeHandler<TOrigin>
    {
        private string value;

        public TextGetRuntimeHandler(string value)
        {
            this.value = value;
        }

        public object Get(TOrigin obj)
        {
            return value;
        }
    }
}