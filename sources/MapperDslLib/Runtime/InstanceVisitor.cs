using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MapperDslLib.Runtime
{
    public class InstanceVisitor<T>
    {
        private string value;
        private PropertyInfo[] navigation;

        public InstanceVisitor(string value)
        {
            this.value = value;
            BuildNavigation();
        }

        public PropertyInfo GetLastPropertyInfo()
        {
            return navigation[navigation.Length - 1];
        }

        private IEnumerable<(object, PropertyInfo)> GetLastPropertyInstance(T obj)
        {
            return Browse(obj, navigation);
        }

        private IEnumerable<(object, PropertyInfo)> Browse(object o, PropertyInfo[] nav)
        {
            var currentValue = o;
            bool reachedEnumerable = false;
            for (int i = 0; i < nav.Length - 1; ++i)
            {
                currentValue = nav[i].GetValue(currentValue);
                if (typeof(IEnumerable).IsAssignableFrom(nav[i].PropertyType))
                {
                    reachedEnumerable = true;
                    foreach (var item in (IEnumerable)currentValue)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        foreach (var subitem in Browse(item, nav.Skip(i + 1).ToArray()))
                        {
                            yield return subitem;
                        }
                    }
                    break;
                }
            }
            if (!reachedEnumerable && currentValue != null)
            {
                yield return (currentValue, navigation[navigation.Length - 1]);
            }
        }

        public void SetInstance(T obj, IEnumerable<object> value)
        {
            var (o, p) = GetLastPropertyInstance(obj).First();
            var v = value.FirstOrDefault();
            if (v is TupleValues values)
            {
                v = values.FirstOrDefault();
            }
            var convertedValue = value == null ? null : Convert.ChangeType(v, p.PropertyType, CultureInfo.InvariantCulture);
            p.SetValue(o, convertedValue);
        }

        public IEnumerable<object> GetInstance(T obj)
        {
            foreach (var (o, p) in GetLastPropertyInstance(obj))
            {
                if (o == null) continue;
                var value = p.GetValue(o);
                if (value == null) continue;
                if (p.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(p.PropertyType)/*.FindInterfaces(new TypeFilter(new Func<Type, object, bool>((t, o) => t == typeof(IEnumerable<>))), null).Any()*/)
                {
                    foreach (var item in (IEnumerable)value)
                    {
                        if (item != null)
                        {
                            yield return item;
                        }
                    }
                }
                else
                {
                    yield return value;
                }
            }
        }

        private void BuildNavigation()
        {
            var navigation = new List<PropertyInfo>();
            var currentType = typeof(T);
            foreach (var identifier in this.value.Split('.'))
            {
                PropertyInfo property;
                (property, currentType) = ModelDescription.GetChild(currentType, identifier);
                navigation.Add(property);
            }
            this.navigation = navigation.ToArray();
        }

    }
}