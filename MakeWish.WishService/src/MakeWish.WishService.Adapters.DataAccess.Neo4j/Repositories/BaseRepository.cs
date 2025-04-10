using MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Repositories;

public abstract class BaseRepository<TEntity>(IServiceProvider serviceProvider) : IBaseRepository<TEntity>
    where TEntity : Entity
{
    private readonly IWriteQueriesStorage _writeQueriesStorage = serviceProvider.GetService<IWriteQueriesStorage>()!;
    
    public virtual void Add(TEntity entity) => AddWriteQuery(NewQuery().Create(entity).Build());
    public virtual void Update(TEntity entity) => AddWriteQuery(NewQuery().Update(entity).Build());
    public virtual void Remove(TEntity entity) => AddWriteQuery(NewQuery().Delete(entity).Build());

    protected IQueryBuilder<TEntity> NewQuery() => serviceProvider.GetService<IQueryBuilder<TEntity>>()!;
    
    protected void AddWriteQuery(string query) => _writeQueriesStorage.Add(query);
    
    protected async Task<List<TEntity>> ExecuteAsync(string query, CancellationToken cancellationToken)
    {
        await using var connection = serviceProvider.GetRequiredService<Neo4jConnection>();
        var mapper = serviceProvider.GetService<IMapper<TEntity>>()!;
        
        var result = await connection.ExecuteReadQueryAsync(query, cancellationToken);
        return result
            .Select(record => mapper.MapToEntity(record))
            .Where(entity => entity is not null)
            .Select(entity => entity!)
            .ToList();
    }
}