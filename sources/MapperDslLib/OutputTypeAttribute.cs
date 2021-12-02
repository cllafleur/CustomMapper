namespace MapperDslLib;
using System;

[System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class OutputTypeAttribute : Attribute
{
    public OutputTypeAttribute(Type outputType)
    {
        OutputType = outputType;
    }

    public Type OutputType { get; private set; }
}
