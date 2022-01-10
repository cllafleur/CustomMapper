using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MapperDslLib.Runtime.Accessor
{
    public interface IInstanceVisitor: IGetterAccessor, ISetterAccessor
    {
    }

    public interface IInstanceVisitor<T> : IInstanceVisitor
    {
    }
}
