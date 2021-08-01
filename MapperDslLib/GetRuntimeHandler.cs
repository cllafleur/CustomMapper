using System.Reflection;

namespace MapperDslLib
{
    internal class GetRuntimeHandler<T> : IGetRuntimeHandler<T>
    {
        private InstanceVisitor<T> instanceVisitor;

        public GetRuntimeHandler(InstanceVisitor<T> instanceVisitor)
        {
            this.instanceVisitor = instanceVisitor;
        }

        public object Get(T obj)
        {
            object instance = instanceVisitor.GetInstance(obj);
            return instance;
        }
    }
}