using System;
using System.Collections.Generic;

namespace MapperDslLib
{
    public class FunctionHandlerProvider : IFunctionHandlerProvider
    {
        private Dictionary<string, FunctionHandlerDescription> registeredTypes = new Dictionary<string, FunctionHandlerDescription>();

        public T Get<T>(string identifier)
            where T : class
        {
            var description = registeredTypes.ContainsKey(identifier) ? registeredTypes[identifier] : null;
            if (description == null)
            {
                return null;
            }
            return (T)description.FunctionHandlerType.GetConstructor(Array.Empty<Type>()).Invoke(Array.Empty<object>());
        }

        public Type GetOutputType(string identifier)
        {
            var description = registeredTypes.ContainsKey(identifier) ? registeredTypes[identifier] : null;
            if (description == null)
            {
                return null;
            }
            return description.OutputType;
        }

        public void Register<T, TImplementation>(string identifier)
            where TImplementation : class, new()
        {
            var description = GetDescription(typeof(TImplementation));
            registeredTypes.Add(identifier, description);
        }

        public void Register<T>(string identifier, Type implementationType)
        {
            var description = GetDescription(implementationType);
            registeredTypes.Add(identifier, description);
        }

        private FunctionHandlerDescription GetDescription(Type functionHandlerType)
        {
            var outputType = functionHandlerType.GetCustomAttributes(typeof(OutputTypeAttribute), false).Cast<OutputTypeAttribute>().FirstOrDefault()?.OutputType;
            return new FunctionHandlerDescription(functionHandlerType, outputType);
        }
    }
}