using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public static class WishesQueryBuilderExtensions
{
    private const string WishType = WishesMapper.WishType;
    private const string UserType = UsersMapper.UserType;
    
    private const string WishNode = WishesMapper.WishNode;
    private const string OwnerNode = WishesMapper.OwnerNode;
    private const string PromiserNode = WishesMapper.PromiserNode;
    private const string CompleterNode = WishesMapper.CompleterNode;
    
    private const string Wish = $"({WishNode})";
    private const string Owner = $"({OwnerNode})";
    private const string Promiser = $"({PromiserNode})";
    private const string Completer = $"({CompleterNode})";

    private const string OwnsLabel = WishesMapper.OwnsLabel;
    private const string PromisedLabel = WishesMapper.PromisedLabel;
    private const string CompletedLabel = WishesMapper.CompletedLabel;
    
    private const string Owns = $"[{OwnsLabel}:{WishesMapper.OwnsLabelType}]";
    private const string Promised = $"[{PromisedLabel}:{WishesMapper.PromisedLabelType}]";
    private const string Completed = $"[{CompletedLabel}:{WishesMapper.CompletedLabelType}]";

    public static IQueryBuilder<T> MatchWish<T>(this IQueryBuilder<T> builder, string node = WishNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishType})");
    
    public static IQueryBuilder<T> MatchWish<T>(this IQueryBuilder<T> builder, Wish wish, string node = WishNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishType} {{ {builder.BuildIdProperty(wish.Id)} }})");
    
    public static IQueryBuilder<T> MatchWish<T>(this IQueryBuilder<T> builder, Guid id, string node = WishNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishType} {{ {builder.BuildIdProperty(id)} }})");
    
    public static IQueryBuilder<T> MatchWish<T>(this IQueryBuilder<T> builder, WishStatus status, string node = WishNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishType} {{ {WishesMapper.StatusProperty}:'{status}' }})");

    public static IQueryBuilder<T> LinkWishToOwner<T>(this IQueryBuilder<T> builder, User owner, string wish = Wish)
        where T : Entity
        => builder.MatchUser(owner, OwnerNode).AppendLine($"MERGE {Owner}-{Owns}->{wish}");
    
    public static IQueryBuilder<T> MatchWishOwner<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.MatchUser(OwnerNode).AppendLine($"MATCH {Owner}-{Owns}->{wish}");
    
    public static IQueryBuilder<T> MatchWishOwner<T>(this IQueryBuilder<T> builder, User owner, string wish = Wish)
        where T : Entity
        => builder.MatchUser(owner, OwnerNode).AppendLine($"MATCH {Owner}-{Owns}->{wish}");

    public static IQueryBuilder<T> LinkWishToPromiser<T>(this IQueryBuilder<T> builder, User promiser, string wish = Wish)
        where T : Entity
        => builder.MatchUser(promiser, PromiserNode).AppendLine($"MERGE {Promiser}-{Promised}->{wish}");
    
    public static IQueryBuilder<T> UnlinkWishFromPromiser<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.MatchWishPromiser(wish).AppendLine($"DELETE {PromisedLabel}");
    
    public static IQueryBuilder<T> MatchWishPromiser<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.MatchUser(PromiserNode).AppendLine($"MATCH {Promiser}-{Promised}->{wish}");
    
    public static IQueryBuilder<T> MatchWishPromiser<T>(this IQueryBuilder<T> builder, User promiser, string wish = Wish)
        where T : Entity
        => builder.MatchUser(promiser, PromiserNode).AppendLine($"MATCH {Promiser}-{Promised}->{wish}");

    public static IQueryBuilder<T> OptionalMatchWishPromiser<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.AppendLine($"OPTIONAL MATCH ({PromiserNode}:{UserType})-{Promised}->{wish}");

    public static IQueryBuilder<T> LinkWishToCompleter<T>(this IQueryBuilder<T> builder, User completer, string wish = Wish)
        where T : Entity
        => builder.MatchUser(completer, CompleterNode).AppendLine($"MERGE {Completer}-{Completed}->{wish}");
    
    public static IQueryBuilder<T> UnlinkWishFromCompleter<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.MatchWishCompleter(wish).AppendLine($"DELETE {CompletedLabel}");
    
    public static IQueryBuilder<T> MatchWishCompleter<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.MatchUser(CompleterNode).AppendLine($"MATCH {Completer}-{Completed}->{wish}");
    
    public static IQueryBuilder<T> OptionalMatchWishCompleter<T>(this IQueryBuilder<T> builder, string wish = Wish)
        where T : Entity
        => builder.AppendLine($"OPTIONAL MATCH ({CompleterNode}:{UserType})-{Completed}->{wish}");
}