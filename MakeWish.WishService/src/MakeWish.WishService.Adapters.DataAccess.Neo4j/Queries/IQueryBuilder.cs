using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public interface IQueryBuilder<in T> where T : Entity
{
    IQueryBuilder<T> AppendLine(string line);
    string Build();
    
    IQueryBuilder<T> Create(T entity);
    IQueryBuilder<T> Update(T entity);
    IQueryBuilder<T> Delete(T entity);

    string BuildIdProperty(Guid id);
}