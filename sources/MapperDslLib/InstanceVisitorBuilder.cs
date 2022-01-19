using System.Collections;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib
{
    internal partial class InstanceVisitorBuilder
    {
        private static readonly IGetAccessorFactoryHandler[] getterAccessorGetBuilders = new IGetAccessorFactoryHandler[]
        {
            new DictionaryGetAccessorFactoryHandler(),
            new EnumerableGetAccessorFactoryHandler(),
            new FieldGetAccessorFactoryHandler(),
        };

        private static readonly ISetAccessorFactoryHandler[] setterAccessorSetBuilders = new ISetAccessorFactoryHandler[]
        {
            new DictionarySetAccessorFactoryHandler(),
            new EnumerableSetAccessorFactoryHandler(),
            new FieldSetAccessorFactoryHandler(),
        };

        public static IGetterAccessor GetGetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            return GetGetterAccessor(typeof(T), children, propertyResolver);
        }

        public static IGetterAccessor GetGetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            var factory = new GetAccessorFactory(getterAccessorGetBuilders);
            return factory.GetGetterAccessor(startType, children, propertyResolver);
        }

        public static ISetterAccessor GetSetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> fields, IPropertyResolverHandler propertyResolver)
        {
            return GetSetterAccessor(typeof(T), fields, propertyResolver);
        }

        public static ISetterAccessor GetSetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> fields, IPropertyResolverHandler propertyResolver)
        {
            var getFactory = new GetAccessorFactory(getterAccessorGetBuilders);
            var setFactory = new SetAccessorFactory(getFactory, setterAccessorSetBuilders);
            return setFactory.GetSetterAccessor(startType, fields, propertyResolver);
        }
    }
}