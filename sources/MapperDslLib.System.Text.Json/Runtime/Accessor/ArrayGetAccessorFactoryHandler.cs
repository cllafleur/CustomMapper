using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace MapperDslLib.Runtime.Accessor
{
    internal class ArrayGetAccessorFactoryHandler : IGetAccessorFactoryHandler
    {
        public (IGetAccessor getter, Type nextType) Create(FieldInfos fieldInfos, Type nextType)
        {
            return (new ArrayInstanciateGetAccessor(fieldInfos), nextType);
        }

        public (bool isTargetedType, Type nextType) DoesHandle(FieldInfos fieldInfos)
        {
            return (fieldInfos.IsArray, null);
        }
    }
}
