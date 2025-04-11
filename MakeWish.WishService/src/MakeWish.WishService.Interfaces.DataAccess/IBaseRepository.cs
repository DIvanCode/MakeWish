using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IBaseRepository<in TEntity> where TEntity : Entity
{
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}