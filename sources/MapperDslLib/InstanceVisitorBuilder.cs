using System.Collections;
using System.Reflection;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib
{
    internal class InstanceVisitorBuilder
    {
        public static IGetterAccessor GetGetterAccessor<T>(IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            return GetGetterAccessor(typeof(T), children, propertyResolver);
        }

        public static IGetterAccessor GetGetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
        {
            bool goNext = false;
            Type currentType = startType;
            IGetterAccessor firstgetter = null;
            IGetterAccessor getter = null;

            foreach (var field in children)
            {
                do
                {
                    var previousGetter = getter;
                    (getter, currentType, goNext) = BuildGetAccessor(currentType, field);
                    if (previousGetter != null)
                    {
                        previousGetter.AddNext(getter);
                    }
                    firstgetter ??= getter;
                }
                while (!goNext);
            }
            return firstgetter;
        }

        private static (IGetterAccessor getter, Type nextType, bool goNext) BuildGetAccessor(Type currentType, FieldInstanceRefMapper field)
        {
            var (isDictionary, nextTypeDic) = CheckIsDictionary(currentType);
            if (isDictionary)
            {
                return (new DictionaryGetterAccessor(currentType, field.Value), nextTypeDic, CheckCanGoNext(nextTypeDic));
            }

            var (mustIterate, nextType) = CheckMustIterate(currentType);
            if (mustIterate)
            {
                return (new EnumeratorGetterAccessor(currentType), nextType, CheckCanGoNext(nextType));
            }

            var prop = currentType.GetProperty(field.Value);
            return (new FieldGetterAccessor(currentType, prop), prop.PropertyType, CheckCanGoNext(prop.PropertyType));

            (bool canGoNext, Type nextType) CheckIsDictionary(Type type)
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
            (bool canGoNext, Type nextType) CheckMustIterate(Type type)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    Type nextType = null;
                    if (type.IsGenericType)
                    {
                        nextType = type.GenericTypeArguments[0];
                    }
                    else if (currentType.IsArray)
                    {
                        nextType = type.GetElementType();
                    }
                    return (true, nextType);
                }
                return (false, null);
            }
            bool CheckCanGoNext(Type type)
            {
                return type == typeof(string) || typeof(IDictionary).IsAssignableFrom(type) || !typeof(IEnumerable).IsAssignableFrom(type);
            }
        }

        class FieldGetterAccessor : IGetterAccessor
        {
            private PropertyInfo prop;
            private Type type;
            private IGetterAccessor getter;

            public FieldGetterAccessor(Type type, PropertyInfo prop)
            {
                this.type = type;
                this.prop = prop;
            }

            public void AddNext(IGetterAccessor getter)
            {
                this.getter = getter;
            }

            public IEnumerable<object> GetInstance(object obj)
            {
                if (obj != null)
                {
                    if (getter == null)
                    {
                        return new object[] { prop.GetValue(obj) };
                    }
                    else
                    {
                        return getter.GetInstance(prop.GetValue(obj));
                    }
                }
                return Enumerable.Empty<object>();
            }

            public PropertyInfo GetPropertyInfo()
            {
                return prop;
            }
        }

        class DictionaryGetterAccessor : IGetterAccessor
        {
            private Type type;
            private string key;
            private IGetterAccessor getter;

            public DictionaryGetterAccessor(Type type, string key)
            {
                this.type = type;
                this.key = key;
            }

            public void AddNext(IGetterAccessor getter)
            {
                this.getter = getter;
            }

            public IEnumerable<object> GetInstance(object obj)
            {
                if (obj == null)
                {
                    return Enumerable.Empty<object>();
                }
                if (getter != null)
                {
                    return getter.GetInstance(((IDictionary)obj)[key]);
                }
                else
                {
                    return new[] { ((IDictionary)obj)[key] };
                }
            }

            public PropertyInfo GetPropertyInfo()
            {
                return null;
            }
        }

        class EnumeratorGetterAccessor : IGetterAccessor
        {
            private Type type;
            private IGetterAccessor getter;

            public EnumeratorGetterAccessor(Type type)
            {
                this.type = type;
            }
            public void AddNext(IGetterAccessor getter)
            {
                this.getter = getter;
            }

            public IEnumerable<object> GetInstance(object obj)
            {
                if (obj == null)
                {
                    return Enumerable.Empty<object>();
                }
                if (getter == null)
                {
                    return (IEnumerable<object>)obj;
                }
                return CreateResult(obj);

                IEnumerable<object> CreateResult(object obj)
                {
                    foreach (var i in (IEnumerable)obj)
                    {
                        foreach (var o in getter.GetInstance(i))
                        {
                            yield return o;
                        }
                    }
                }
            }

            public PropertyInfo GetPropertyInfo()
            {
                return null;
            }
        }
    }
}