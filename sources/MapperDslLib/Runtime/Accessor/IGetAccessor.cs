namespace MapperDslLib.Runtime.Accessor;

interface IGetAccessor
{
    IEnumerable<object> GetInstance(object obj);
    IGetAccessor Next { get; set; }
}
