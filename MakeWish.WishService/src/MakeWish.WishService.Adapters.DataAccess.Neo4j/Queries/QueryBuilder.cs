using System.Text;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public sealed class QueryBuilder<T>(IMapper<T> mapper) : IQueryBuilder<T> where T : Entity
{
    private readonly StringBuilder _builder = new();
    private readonly List<string> _params = [];

    public IQueryBuilder<T> AppendLine(string line)
    {
        var node = ExtractNode(line);
        if (node is not null)
        {
            _params.Add(node);
        }
        
        _builder.AppendLine(line);
        
        return this;
    }

    public string Build()
    {
        var properties = mapper.GetReturningRecordProperties(mapper.EntityNode, _params.ToArray());
        _params.Clear();

        this.Return(string.Join(", ", properties));
        
        var query = _builder.ToString();
        _builder.Clear();
        
        return query;
    }
    
    public IQueryBuilder<T> Create(T entity)
    {
        var properties = BuildPropertiesMatchingString(entity);
        return this.Create(mapper.EntityNode, mapper.EntityType, properties);
    }
    
    public IQueryBuilder<T> Update(T entity)
    {
        var matchProperties = BuildIdProperty(entity.Id);
        var properties = BuildPropertiesSettingString(mapper.EntityNode, entity);
        return this.Update(mapper.EntityNode, mapper.EntityType, matchProperties, properties);
    }
    
    public IQueryBuilder<T> Delete(T entity)
    {
        var properties = BuildIdProperty(entity.Id);
        return this.Delete(mapper.EntityNode, mapper.EntityType, properties);
    }
    
    public string BuildIdProperty(Guid id) => $"{IMapper<T>.IdProperty}:'{id}'";
    
    public override string ToString() => throw new NotSupportedException();
    
    private static string? ExtractNode(string line)
    {
        if (!line.Contains(':'))
        {
            return null;
        }
        
        line = line[..line.IndexOf(':')];
        return line[(line.LastIndexOf('(')+1)..];
    }
    
    private string BuildPropertiesMatchingString(T entity)
    {
        var properties = mapper.GetEntityPropertiesDictionary(entity);
        return string.Join(", ", properties.Select(p => $"{p.Key}:'{p.Value}'"));   
    }
    
    private string BuildPropertiesSettingString(string node, T entity)
    {
        var properties = mapper.GetEntityPropertiesDictionary(entity);
        return string.Join(", ", properties.Select(p => $"{node}.{p.Key} = '{p.Value}'"));   
    }
}