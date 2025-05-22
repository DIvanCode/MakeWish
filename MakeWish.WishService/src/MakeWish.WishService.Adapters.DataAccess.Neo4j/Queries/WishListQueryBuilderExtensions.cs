using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;

public static class WishListQueryBuilderExtensions
{
    private const string WishListType = WishListsMapper.WishListType;
    private const string WishListNode = WishListsMapper.WishListNode;

    private const string OwnerNode = WishListsMapper.OwnerNode;
    private const string WishNode = WishListsMapper.WishNode;
    private const string UserNode = WishListsMapper.UserNode;
    
    private const string WishList = $"({WishListsMapper.WishListNode})";
    private const string Owner = $"({OwnerNode})";
    private const string Wish = $"({WishNode})";
    private const string User = $"({UserNode})";

    private const string OwnsLabel = WishListsMapper.OwnsLabel;
    private const string ContainsLabel = WishListsMapper.ContainsLabel;
    private const string HasAccessLabel = WishListsMapper.HasAccessLabel;
    
    private const string Owns = $"[{OwnsLabel}:{WishListsMapper.OwnsLabelType}]";
    private const string Contains = $"[{ContainsLabel}:{WishListsMapper.ContainsLabelType}]";
    private const string HasAccess = $"[{HasAccessLabel}:{WishListsMapper.HasAccessLabelType}]";
    
    public static IQueryBuilder<T> MatchWishList<T>(this IQueryBuilder<T> builder, string node = WishListNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishListType})");
    
    public static IQueryBuilder<T> MatchWishList<T>(this IQueryBuilder<T> builder, WishList wishList, string node = WishListNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishListType} {{ {builder.BuildIdProperty(wishList.Id)} }})");
    
    public static IQueryBuilder<T> MatchWishList<T>(this IQueryBuilder<T> builder, Guid id, string node = WishListNode)
        where T : Entity
        => builder.AppendLine($"MATCH ({node}:{WishListType} {{ {builder.BuildIdProperty(id)} }})");
    
    public static IQueryBuilder<T> LinkWishListToOwner<T>(this IQueryBuilder<T> builder, User owner, string wishList = WishList)
        where T : Entity
        => builder.MatchUser(owner, OwnerNode).AppendLine($"MERGE {Owner}-{Owns}->{wishList}");
    
    public static IQueryBuilder<T> MatchWishListOwner<T>(this IQueryBuilder<T> builder, string wishList = WishList)
        where T : Entity
        => builder.MatchUser(OwnerNode).AppendLine($"MATCH {Owner}-{Owns}->{wishList}");
    
    public static IQueryBuilder<T> MatchWishListOwner<T>(this IQueryBuilder<T> builder, User owner, string wishList = WishList)
        where T : Entity
        => builder.MatchUser(owner, OwnerNode).AppendLine($"MATCH {Owner}-{Owns}->{wishList}");
    
    public static IQueryBuilder<T> MatchWishListContainsWish<T>(this IQueryBuilder<T> builder, Wish wish, string wishList = WishList)
        where T : Entity
        => builder.MatchWish(wish).AppendLine($"MATCH {wishList}-{Contains}->{Wish}");
    
    public static IQueryBuilder<T> LinkWishToWishList<T>(this IQueryBuilder<T> builder, Wish wish, string wishList = WishList)
        where T : Entity
        => builder.MatchWish(wish).AppendLine($"MERGE {wishList}-{Contains}->{Wish}");
    
    public static IQueryBuilder<T> UnlinkWishFromWishList<T>(this IQueryBuilder<T> builder, Wish wish, string wishList = WishList)
        where T : Entity
        => builder.MatchWishListContainsWish(wish, wishList).AppendLine($"DELETE {ContainsLabel}");
    
    public static IQueryBuilder<T> MatchWishListWishes<T>(this IQueryBuilder<T> builder, string wishList = WishList)
        where T : Entity
        => builder
            .MatchWish()
            .MatchWishOwner()
            .OptionalMatchWishPromiser()
            .OptionalMatchWishCompleter()
            .AppendLine($"MATCH {wishList}-{Contains}->{Wish}");
    
    public static IQueryBuilder<T> MatchUserAccessToWishList<T>(this IQueryBuilder<T> builder, User user, string wishList = WishList) 
        where T : Entity
        => builder.MatchUser(user).AppendLine($"MATCH {User}-{HasAccess}->{wishList}");
    
    public static IQueryBuilder<T> MatchUserAccessToWishList<T>(this IQueryBuilder<T> builder, string wishList = WishList) 
        where T : Entity
        => builder.MatchUser().AppendLine($"MATCH {User}-{HasAccess}->{wishList}");
    
    public static IQueryBuilder<T> LinkUserAccessToWishList<T>(this IQueryBuilder<T> builder, User user, string wishList = WishList)
        where T : Entity
        => builder.MatchUser(user).AppendLine($"MERGE {User}-{HasAccess}->{wishList}");
    
    public static IQueryBuilder<T> UnlinkUserAccessFromWishList<T>(this IQueryBuilder<T> builder, User user, string wishList = WishList)
        where T : Entity
        => builder.MatchUserAccessToWishList(user, wishList).AppendLine($"DELETE {HasAccessLabel}");
        
}