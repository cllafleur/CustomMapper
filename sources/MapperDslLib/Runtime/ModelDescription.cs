using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperDslLib.Runtime
{
    internal class ModelDescription
    {
        private static Dictionary<Type, Dictionary<string, (PropertyInfo, Type)>> cache = new Dictionary<Type, Dictionary<string, (PropertyInfo, Type)>>();

        public static (PropertyInfo, Type) GetChild(Type type, string identifier, IPropertyResolverHandler propertyHandler)
        {
            if (!Settings.EnableReflectionCaching)
            {
                return GetChildType(type, identifier, propertyHandler);
            }
            (PropertyInfo, Type) result;
            if (cache.TryGetValue(type, out var typeInfo))
            {
                if (!typeInfo.TryGetValue(identifier, out result))
                {
                    result = GetChildType(type, identifier, propertyHandler);
                    cache[type].Add(identifier, result);
                }
            }
            else
            {
                result = GetChildType(type, identifier, propertyHandler);
                cache.Add(type, new Dictionary<string, (PropertyInfo, Type)> { { identifier, result } });
            }
            return result;
        }

        private static (PropertyInfo, Type) GetChildType(Type currentType, string identifier, IPropertyResolverHandler propertyHandler)
        {
            while (typeof(IEnumerable).IsAssignableFrom(currentType))
            {
                if (currentType.IsGenericType)
                {
                    currentType = currentType.GenericTypeArguments[0];
                }
                else if (currentType.IsArray)
                {
                    currentType = currentType.GetElementType();
                }
                else
                {
                    break;
                }
            }
            try
            {
                var property = propertyHandler.GetProperty(currentType, identifier);
                if (property == null)
                {
                    throw new MapperVisitException($"Property '{identifier}' not found");
                }
                currentType = property.PropertyType;
                return (property, currentType);
            }
            catch (Exception exc)
            {
                throw new MapperVisitException($"Property '{identifier}' not found", exc);
            }
        }

    }
}
