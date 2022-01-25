namespace MapperDslLib.Parser;

public class ArrayFieldInstanceRefMapper : FieldInstanceRefMapper
{
    public bool IsArray => true;

    public ArrayFieldInstanceRefMapper(string value, ParsingInfo infos) : base(value, infos) { }
}
