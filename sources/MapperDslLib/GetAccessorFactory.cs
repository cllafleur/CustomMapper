using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MapperDslLib.Parser;
using MapperDslLib.Runtime;
using MapperDslLib.Runtime.Accessor;

namespace MapperDslLib;

internal class GetAccessorFactory
{
    private readonly IGetAccessorFactoryHandler[] getterAccessorGetBuilders;
    private readonly IGetAccessor deconstructorAccessor;

    public GetAccessorFactory(IGetAccessorFactoryHandler[] getterAccessorGetBuilders, IGetAccessor deconstructorAccessor)
    {
        this.getterAccessorGetBuilders = getterAccessorGetBuilders;
        this.deconstructorAccessor = deconstructorAccessor;
    }

    public IGetterAccessor GetGetterAccessor(Type startType, IEnumerable<FieldInstanceRefMapper> children, IPropertyResolverHandler propertyResolver)
    {
        var (getter, _) = GetGetAccessorImpl(startType, children, propertyResolver, getterAccessorGetBuilders);
        return new GetterAccessor(getter, deconstructorAccessor);
    }

    public (IGetAccessor, Type) GetGetAccessor(
            Type startType,
            IEnumerable<FieldInstanceRefMapper> children,
            IPropertyResolverHandler propertyResolver,
            bool stopBeforeLast = false)
    {
        return GetGetAccessorImpl(startType, children, propertyResolver, getterAccessorGetBuilders, stopBeforeLast);
    }

    private static (IGetAccessor, Type) GetGetAccessorImpl(
        Type startType,
        IEnumerable<FieldInstanceRefMapper> children,
        IPropertyResolverHandler propertyResolver,
        IGetAccessorFactoryHandler[] builders,
        bool stopBeforeLast = false)
    {
        Type currentType = startType;
        var accessors = new List<IGetAccessor>();
        IGetAccessor firstAccessor = null;
        IGetAccessor getter = null;
        int count = 0;
        bool dropFirstAccessorWhenStopBeforeLast = true;

        foreach (var field in children)
        {
            count++;
            bool goNext = false;
            do
            {
                var previousAccessor = getter;
                var previousType = currentType;
                (getter, currentType) = BuildGetAccessor(currentType, field, builders);
                goNext = currentType == null ? true : CheckCanGoNext(currentType);
                firstAccessor ??= getter;
                if (stopBeforeLast && count == children.Count() && goNext)
                {
                    currentType = previousType;
                    firstAccessor = dropFirstAccessorWhenStopBeforeLast ? null : firstAccessor;
                    break;
                }
                if (previousAccessor != null)
                {
                    previousAccessor.Next = getter;
                }
                dropFirstAccessorWhenStopBeforeLast = false;
            }
            while (!goNext);
        }
        return (firstAccessor, currentType);
    }

    private static (IGetAccessor getter, Type nextType) BuildGetAccessor(
                Type currentType,
                FieldInstanceRefMapper field,
                IGetAccessorFactoryHandler[] builders)
    {

        foreach (var i in builders)
        {
            var fieldInfos = new FieldInfos { OutputType = currentType, Identifier = field.Value, IsArray = field is ArrayFieldInstanceRefMapper };
            var (isTargetedType, nextType) = i.DoesHandle(fieldInfos);
            if (isTargetedType)
            {
                return i.Create(fieldInfos, nextType);
            }
        }

        throw new NotSupportedException($"{currentType}");
    }

    private static bool CheckCanGoNext(Type type)
    {
        return !typeof(IEnumerable).IsAssignableFrom(type) || type == typeof(string) || typeof(IDictionary).IsAssignableFrom(type)
            || type.GetInterfaces().Where(t => t.GUID == typeof(IDictionary<,>).GUID).Any();
    }
}
