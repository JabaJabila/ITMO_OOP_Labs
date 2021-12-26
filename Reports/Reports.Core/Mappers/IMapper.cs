namespace Core.Mappers
{
    public interface IMapper<out TDto, in TEntity>
    {
        TDto Map(TEntity entity);
    }
}