using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Repositories;

public sealed class WishListsRepository(IServiceProvider serviceProvider)
    : BaseRepository<WishList>(serviceProvider), IWishListsRepository
{
    public override void Add(WishList wishList)
    {
        base.Add(wishList);

        var query = NewQuery()
            .MatchWishList(wishList)
            .LinkWishListToOwner(wishList.Owner);
        AddWriteQuery(query.Build());
    }

    public void AddWish(WishList wishList, Wish wish)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .LinkWishToWishList(wish);
        AddWriteQuery(query.Build());
    }
    
    public void RemoveWish(WishList wishList, Wish wish)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .UnlinkWishFromWishList(wish);
        AddWriteQuery(query.Build());
    }
    
    public async Task<WishList?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var wishListQuery = NewQuery()
            .MatchWishList(id)
            .MatchWishListOwner();
        var wishListResult = await ExecuteAsync(wishListQuery.Build(), cancellationToken);
        var wishList = wishListResult.SingleOrDefault();

        if (wishList is null)
        {
            return wishList;   
        }

        var wishListWishesQuery = NewQuery()
            .MatchWishList(id)
            .MatchWishListWishes();
        var wishListWishesResult = await ExecuteAsync(wishListWishesQuery.Build(), cancellationToken);
        wishListWishesResult.ForEach(wl => wl.Wishes.ToList().ForEach(w => AddWishToWishList(wishList, w)));
        
        return wishList;
    }

    public async Task<bool> HasUserAccessAsync(WishList wishList, User user, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .MatchUserAccessToWishList(user);
        var result = await ExecuteAsync(query.Build(), cancellationToken);
        return result.Count != 0;
    }
    
    public async Task<bool> ExistsContainingWishWithUserAccessAsync(Wish wish, User user, CancellationToken cancellationToken)
    {
        var query = NewQuery().MatchWishList()
            .MatchWishListContainsWish(wish)
            .MatchUserAccessToWishList(user);
        var result = await ExecuteAsync(query.Build(), cancellationToken);
        return result.Count != 0;
    }
    
    public void AllowUserAccess(WishList wishList, User user, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .LinkUserAccessToWishList(user);
        AddWriteQuery(query.Build());
    }
    
    public void DenyUserAccess(WishList wishList, User user, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .UnlinkUserAccessFromWishList(user);
        AddWriteQuery(query.Build());
    }
    
    public async Task<List<WishList>> GetWishListsWithOwnerAsync(User owner, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWishList()
            .MatchWishListOwner(owner);
        var result = await ExecuteAsync(query.Build(), cancellationToken);
        return result.ToList();
    }

    private static void AddWishToWishList(WishList wishList, Wish wish)
        => ReflectionHelper<WishList>.AddEntryToListPropertyValue(wishList, WishListsMapper.WishesProperty, wish, extractValue: false);
} 