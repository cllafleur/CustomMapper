using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    internal class FieldGetAccessorFactoryHandler : IGetAccessorFactoryHandler
    {
        public (IGetAccessor getter, Type nextType) Create(string identifier, Type currentType)
        {
            var prop = currentType.GetProperty(identifier);
            return (new FieldGetAccessor(prop), prop.PropertyType);
        }

        public (bool isTargetedType, Type nextType) DoesHandle(Type type)
        {
            return (true, type);
        }
    }
}
