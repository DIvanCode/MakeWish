using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public static class QueryBuilderExtensions
{
    public static IQueryBuilder<T> Match<T>(this IQueryBuilder<T> builder, string node, string type, string prop) where T : Entity
        => builder.AppendLine($"MATCH ({node}:{type} {{ {prop} }})");
    
    public static IQueryBuilder<T> Create<T>(this IQueryBuilder<T> builder, string node, string type, string prop) where T : Entity
        => builder.AppendLine($"CREATE ({node}:{type} {{ {prop} }})");
    
    public static IQueryBuilder<T> Update<T>(this IQueryBuilder<T> builder, string node, string type, string matchProp, string prop) where T : Entity
        => builder
            .Match(node, type, matchProp)
            .AppendLine($"SET {prop}");
    
    public static IQueryBuilder<T> Delete<T>(this IQueryBuilder<T> builder, string node, string type, string prop) where T : Entity
        => builder
            .Match(node, type, prop)
            .AppendLine($"DETACH DELETE {node}");
    
    public static void Return<T>(this IQueryBuilder<T> builder, string propertiesMapping) where T : Entity
        => builder.AppendLine($"RETURN {propertiesMapping}");
}