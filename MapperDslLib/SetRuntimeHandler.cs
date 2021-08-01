namespace MapperDslLib
{
    internal class SetRuntimeHandler<T> : ISetRuntimeHandler<T>
    {
        private InstanceVisitor<T> instanceVisitor;

        public SetRuntimeHandler(InstanceVisitor<T> instanceVisitor)
        {
            this.instanceVisitor = instanceVisitor;
        }

        public void SetValue(T obj, object value)
        {
         instanceVisitor.SetInstance(obj, value);
        }
    }
}