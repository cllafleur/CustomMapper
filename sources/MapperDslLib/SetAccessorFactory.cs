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
            var lastField = fields.Last();
            var fieldInfos = new FieldInfos { Identifier = lastField.Value, IsArray = lastField is ArrayFieldInstanceRefMapper, OutputType = outputType };
            var (isTarget, _) = handler.DoesHandle(fieldInfos);
            if (isTarget)
            {
                return handler.Create(outputType, getAccessor, fieldInfos);
            }
        }
        throw new NotSupportedException($"{outputType}");
    }
}
