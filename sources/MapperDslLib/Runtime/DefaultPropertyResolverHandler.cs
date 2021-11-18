namespace MapperDslLib.Runtime;
using System.Reflection;

public class DefaultPropertyResolverHandler : IPropertyResolverHandler
{
    public static readonly IPropertyResolverHandler Instance = new DefaultPropertyResolverHandler(); 

    public PropertyInfo GetProperty(Type objectType, string name)
    {
        return objectType.GetProperty(name);
    }
}