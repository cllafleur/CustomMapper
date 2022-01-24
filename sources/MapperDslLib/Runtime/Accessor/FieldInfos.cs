namespace MapperDslLib.Runtime.Accessor;

public class FieldInfos
{
    public string Identifier { get; set; }
    public bool IsArray { get; set; }
    public Type OutputType { get; internal set; }
}