namespace MapperDslLib.Runtime;
using System.Reflection;

public interface IPropertyResolverHandler
{
    PropertyInfo GetProperty(Type objectType, string name);
}