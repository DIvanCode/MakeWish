using MakeWish.WishService.Models;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;

public interface IMapper<T> where T : Entity
{
    public const string IdProperty = "Id";
    
    string EntityType { get; }
    string EntityNode { get; }

    Dictionary<string, string> GetEntityPropertiesDictionary(T entity);
    List<string> GetReturningRecordProperties(string node, params string[] neighbours);

    T? MapToEntity(IRecord record, string? node = null);
}