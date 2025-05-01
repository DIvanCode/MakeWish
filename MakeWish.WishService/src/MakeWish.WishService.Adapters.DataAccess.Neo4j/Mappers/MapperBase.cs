using MakeWish.WishService.Models;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;

public abstract class MapperBase<TEntity> : IMapper<TEntity> where TEntity : Entity
{
    public abstract string EntityType { get; }
    public abstract string EntityNode { get; }

    protected abstract List<string> Properties { get; }
    
    public virtual List<string> GetReturningRecordProperties(string node, params string[] neighbours)
        => Properties.Select(name => $"{node}.{name} AS {node}_{name}").ToList();

    public virtual TEntity? MapToEntity(IRecord record, string? node = null)
        => ConstructEntity(Activator.CreateInstance<TEntity>(), record, node);
    
    public Dictionary<string, string> GetEntityPropertiesDictionary(TEntity entity)
    {
        var properties = new Dictionary<string, string>();
        foreach (var fieldInfo in ReflectionHelper<TEntity>.GetFieldsInfo())
        {
            var property = ReflectionHelper.ExtractFieldNameFromInfo(fieldInfo);
            if (!Properties.Contains(property))
            {
                continue;
            }

            var value = fieldInfo.GetValue(entity) ?? "";
            properties[property] = value.ToString()!;
        }

        return properties;
    }
    
    private TEntity? ConstructEntity(TEntity entity, IRecord record, string? node)
    {
        node ??= EntityNode;

        if (Properties.All(property => !record.Keys.Contains($"{node}_{property}") || record[$"{node}_{property}"] is null))
        {
            return null;
        }
        
        foreach (var property in Properties)
        {
            ReflectionHelper<TEntity>.SetPropertyValue(entity, property, record[$"{node}_{property}"]);
        }

        return entity;
    }
}