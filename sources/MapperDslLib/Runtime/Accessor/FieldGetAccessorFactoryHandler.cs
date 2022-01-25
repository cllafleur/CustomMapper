using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    internal class FieldGetAccessorFactoryHandler : IGetAccessorFactoryHandler
    {
        public (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type currentType)
        {
            var prop = currentType.GetProperty(fieldInfos.Identifier);
            return (new FieldGetAccessor(prop), prop.PropertyType);
        }

        public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
        {
            return (true, fieldInfos.OutputType);
        }
    }
}
