namespace MapperDslLib.Runtime.Accessor;

public interface IGetAccessor
{
    IEnumerable<object> GetInstance(object obj);
    IGetAccessor Next { get; set; }
}
