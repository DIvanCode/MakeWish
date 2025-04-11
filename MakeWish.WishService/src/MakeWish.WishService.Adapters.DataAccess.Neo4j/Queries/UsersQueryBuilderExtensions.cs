using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public static class UsersQueryBuilderExtensions
{
    private const string UserType = UsersMapper.UserType;
    private const string UserNode = UsersMapper.UserNode;
    
    private const string User = $"({UsersMapper.UserNode})";
    
    public static IQueryBuilder<T> MatchUser<T>(this IQueryBuilder<T> builder, string node = UserNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{UserType})");
    
    public static IQueryBuilder<T> MatchUser<T>(this IQueryBuilder<T> builder, Guid id, string node = UserNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{UserType} {{ {builder.BuildIdProperty(id)} }})");
    
    public static IQueryBuilder<T> MatchUser<T>(this IQueryBuilder<T> builder, User user, string node = UserNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{UserType} {{ {builder.BuildIdProperty(user.Id)} }})");
}