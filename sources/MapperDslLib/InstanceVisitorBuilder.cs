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

        public static IGetterAccessor GetGetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver, CompilerOptions options = null)
        {
            return GetGetterAccessor(typeof(T), children, propertyResolver, options);
        }

        public static IGetterAccessor GetGetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver, CompilerOptions options = null)
        {
            var factory = new GetAccessorFactory(options?.ExtractGetAccessorFactoryHandlers ?? getterAccessorGetBuilders, options?.DeconstructorAccessor);
            return factory.GetGetterAccessor(startType, children, propertyResolver);
        }

        public static ISetterAccessor GetSetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> fields, IPropertyResolverHandler propertyResolver, CompilerOptions options = null)
        {
            return GetSetterAccessor(typeof(T), fields, propertyResolver, options);
        }

        public static ISetterAccessor GetSetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> fields, IPropertyResolverHandler propertyResolver, CompilerOptions options = null)
        {
            var getFactory = new GetAccessorFactory(options?.InsertGetAccessorFactoryHandlers ?? getterAccessorGetBuilders, null);
            var setFactory = new SetAccessorFactory(getFactory, options?.InsertSetAccessorFactoryHandlers ?? setterAccessorSetBuilders);
            return setFactory.GetSetterAccessor(startType, fields, propertyResolver);
        }
    }
}