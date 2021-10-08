using System;
using System.Collections.Generic;

namespace MapperDslLib
{
    public class FunctionHandlerProvider : IFunctionHandlerProvider
    {
        private Dictionary<string, Type> registeredTypes = new Dictionary<string, Type>();

        public T Get<T>(string identifier)
            where T : class
        {
            var type = registeredTypes.ContainsKey(identifier) ? registeredTypes[identifier] : null;
            if (type == null)
            {
                return null;
            }
            return (T)type.GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>());
        }

        public void Register<T, TImplementation>(string identifier)
            where TImplementation : class, new()
        {
            registeredTypes.Add(identifier, typeof(TImplementation));
        }

        public void Register<T>(string identifier, Type implementationType)
        {
            registeredTypes.Add(identifier, implementationType);
        }
    }
}