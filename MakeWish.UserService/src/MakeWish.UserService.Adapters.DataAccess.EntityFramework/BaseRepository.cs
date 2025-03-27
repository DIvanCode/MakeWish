using MakeWish.UserService.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework;

public abstract class BaseRepository<TEntity>(DbSet<TEntity> entities) : IBaseRepository<TEntity> where TEntity : class
{
    public void Add(TEntity entity)
    {
        entities.Add(entity);
    }

    public void Update(TEntity entity)
    {
        entities.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        entities.Remove(entity);
    }
}