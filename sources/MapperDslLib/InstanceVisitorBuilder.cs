using System.Collections;
using System.Globalization;
using System.Reflection;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib
{
    internal partial class InstanceVisitorBuilder
    {
        private static readonly (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[] getterAccessorGetBuilders = new (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[]
        {
            (CheckIsDictionary, BuildDictionaryGetAccessor),
            (CheckMustIterate, BuildEnumeratorGetAccessor),
        };

        private static readonly (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[] getterAccessorSetBuilders = new (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[]
        {
            (CheckIsDictionary, BuildDictionaryGetAccessor),
        };

        public static IGetterAccessor GetGetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            return GetGetterAccessor(typeof(T), children, propertyResolver);
        }

        public static IGetterAccessor GetGetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            var (getter, _) = GetGetAccessorImpl(startType, children, propertyResolver, getterAccessorGetBuilders);
            return new GetterAccessor(getter);
        }

        private static (IGetAccessor, Type) GetGetAccessorImpl(
            Type startType,
            IEnumerable<FieldInstanceRefMapper> children,
            IPropertyResolverHandler propertyResolver,
            (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[] builders)
        {
            Type currentType = startType;
            var accessors = new List<IGetAccessor>();
            IGetAccessor firstAccessor = null;
            IGetAccessor getter = null;

            foreach (var field in children)
            {
                bool goNext = false;
                do
                {
                    var previousAccessor = getter;
                    (getter, currentType, goNext) = BuildGetAccessor(currentType, field, builders);
                    if (previousAccessor != null)
                    {
                        previousAccessor.Next = getter;
                    }
                    firstAccessor ??= getter;
                }
                while (!goNext);
            }
            return (firstAccessor, currentType);
        }

        public static ISetterAccessor GetSetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> children, FieldInstanceRefMapper fieldToSet, IPropertyResolverHandler propertyResolver)
        {
            return GetSetterAccessor(typeof(T), children, fieldToSet, propertyResolver);
        }

        public static ISetterAccessor GetSetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, FieldInstanceRefMapper fieldToSet, IPropertyResolverHandler propertyResolver)
        {
            var (getAccessor, outputType) = children != null
                ? GetGetAccessorImpl(startType, children, propertyResolver, getterAccessorSetBuilders)
                : (null, startType);
            return BuildFieldSetAccessor(outputType, getAccessor, fieldToSet);
        }

        class FieldSetterAccessor : ISetterAccessor
        {
            private IGetAccessor accessor;
            private PropertyInfo prop;

            public FieldSetterAccessor(IGetAccessor accessor, PropertyInfo prop)
            {
                this.accessor = accessor;
                this.prop = prop;
            }

            public void SetInstance(object obj, IEnumerable<object> value)
            {
                var o = accessor != null ? accessor.GetInstance(obj).First() : obj;
                var v = value.FirstOrDefault();
                if (v is TupleValues values)
                {
                    v = values.FirstOrDefault();
                }
                object convertedValue = null;
                Type innerType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (innerType != null && innerType == v.GetType())
                {
                    convertedValue = v;
                }
                else
                {
                    convertedValue = value == null ? null : Convert.ChangeType(v, prop.PropertyType, CultureInfo.InvariantCulture);
                }
                prop.SetValue(o, convertedValue);
            }
        }

        private static ISetterAccessor BuildFieldSetAccessor(Type currentType, IGetAccessor accessor, FieldInstanceRefMapper field)
        {
            var prop = currentType.GetProperty(field.Value);
            return new FieldSetterAccessor(accessor, prop);
        }

        private static (IGetAccessor getter, Type nextType, bool goNext) BuildGetAccessor(
            Type currentType,
            FieldInstanceRefMapper field,
            (Func<Type, (bool, Type)>, Func<FieldInstanceRefMapper, Type, (IGetAccessor, Type, bool)>)[] builders)
        {

            foreach (var i in builders)
            {
                var (isTargetedType, nextType) = i.Item1(currentType);
                if (isTargetedType)
                {
                    return i.Item2(field, nextType);
                }
            }

            return BuildFieldGetAccessor(currentType, field);
        }

        private static (IGetAccessor getter, Type nextType, bool goNext) BuildFieldGetAccessor(Type currentType, FieldInstanceRefMapper field)
        {
            var prop = currentType.GetProperty(field.Value);
            return (new FieldGetAccessor(prop), prop.PropertyType, CheckCanGoNext(prop.PropertyType));
        }

        private static (IGetAccessor getter, Type nextType, bool goNext) BuildEnumeratorGetAccessor(FieldInstanceRefMapper field, Type nextType)
        {
            return (new EnumeratorGetAccessor(), nextType, CheckCanGoNext(nextType));
        }

        private static (IGetAccessor getter, Type nextType, bool goNext) BuildDictionaryGetAccessor(FieldInstanceRefMapper field, Type nextTypeDic)
        {
            return (new DictionaryGetAccessor(field.Value), nextTypeDic, CheckCanGoNext(nextTypeDic));
        }

        private static (bool canGoNext, Type nextType) CheckIsDictionary(Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                if (type.IsGenericType)
                {
                    return (true, type.GenericTypeArguments[1]);
                }
                return (true, typeof(object));
            }
            return (false, null);
        }

        private static (bool canGoNext, Type nextType) CheckMustIterate(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                Type nextType = null;
                if (type.IsGenericType)
                {
                    nextType = type.GenericTypeArguments[0];
                }
                else if (type.IsArray)
                {
                    nextType = type.GetElementType();
                }
                return (true, nextType);
            }
            return (false, null);
        }

        private static bool CheckCanGoNext(Type type)
        {
            return type == typeof(string) || typeof(IDictionary).IsAssignableFrom(type) || !typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}