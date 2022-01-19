using System;
using System.Collections.Generic;
using System.Text;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib;

internal class SetAccessorFactory
{
    private readonly GetAccessorFactory getFactory;
    private readonly ISetAccessorFactoryHandler[] setterAccessorSetBuilders;

    public SetAccessorFactory(GetAccessorFactory getFactory, ISetAccessorFactoryHandler[] setterAccessorSetBuilders)
    {
        this.getFactory = getFactory;
        this.setterAccessorSetBuilders = setterAccessorSetBuilders;
    }

    public ISetterAccessor GetSetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> fields, IPropertyResolverHandler propertyResolver)
    {
        var (getAccessor, outputType) = getFactory.GetGetAccessor(startType, fields, propertyResolver, true);
        foreach (var handler in setterAccessorSetBuilders)
        {
            var (isTarget, _) = handler.DoesHandle(outputType);
            if (isTarget)
            {
                return handler.Create(outputType, getAccessor, fields.Last().Value);
            }
        }
        throw new NotSupportedException($"{outputType}");
    }
}
