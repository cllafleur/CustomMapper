namespace MapperDslLib
{
    public interface IMapperHandler<TOrigin, TTarget>
    {
        void Map(TOrigin origin, TTarget target);
    }
}