using MakeWish.WishService.Models;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;

public sealed class WishListsMapper(IMapper<User> usersMapper, IMapper<Wish> wishesMapper) : MapperBase<WishList>
{
    public const string WishListType = nameof(WishList);
    
    public const string WishListNode = "wishlist";
    public const string OwnerNode = "wishlist_owner";
    public const string WishNode = "wish";
    public const string UserNode = "user";

    public const string OwnsLabel = "owns_wishlist";
    public const string OwnsLabelType = "Owns";
    
    public const string ContainsLabel = "contains_wish";
    public const string ContainsLabelType = "Contains";
    
    public const string HasAccessLabel = "has_access_to_wishlist";
    public const string HasAccessLabelType = "HasAccess";
    
    public const string TitleProperty = "Title";
    
    public const string OwnerProperty = "Owner";
    public const string WishesProperty = "_wishes";
    
    public override string EntityType => WishListType;
    public override string EntityNode => WishListNode;

    protected override List<string> Properties => [IMapper<WishList>.IdProperty, TitleProperty];

    public override List<string> GetReturningRecordProperties(string node, params string[] neighbours)
    {
        var properties = base.GetReturningRecordProperties(node);
        
        if (neighbours.Contains(OwnerNode))
            properties = properties.Concat(usersMapper.GetReturningRecordProperties(OwnerNode, neighbours)).ToList();
        if (neighbours.Contains(WishNode))
            properties = properties.Concat(wishesMapper.GetReturningRecordProperties(WishNode, neighbours)).ToList();
        if (neighbours.Contains(UserNode))
            properties = properties.Concat(usersMapper.GetReturningRecordProperties(UserNode, neighbours)).ToList();
        
        return properties;
    }

    public override WishList? MapToEntity(IRecord record, string? node = null)
    {
        var wishList = base.MapToEntity(record, node);
        if (wishList is null)
        {
            return wishList;
        }
        
        var owner = usersMapper.MapToEntity(record, OwnerNode);
        ReflectionHelper<WishList>.SetPropertyValue(wishList, OwnerProperty, owner, extractValue: false);
        
        var wish = wishesMapper.MapToEntity(record, WishNode);
        if (wish is not null)
            ReflectionHelper<WishList>.AddEntryToListPropertyValue(wishList, WishesProperty, wish, extractValue: false);
        
        return wishList;
    }
}