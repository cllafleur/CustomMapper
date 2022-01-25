using System;
using System.Collections.Generic;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    internal class ArraySetAccessorFactoryHandler : ISetAccessorFactoryHandler
    {
        public ISetterAccessor Create(Type outputType, IGetAccessor getAccessor, FieldInfos fieldInfos)
        {
            return new ArraySetAccessor(getAccessor, fieldInfos);
        }

        public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
        {
            return (fieldInfos.IsArray, null);
        }
    }
}
