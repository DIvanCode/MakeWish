using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class WishListsRepositoryStub(GlobalStorage globalStorage) : IWishListsRepository
{
    private List<WishList> WishLists => globalStorage.WishLists;
    private Dictionary<WishList, List<User>> WishListUserAccess => globalStorage.WishListUserAccess;

    public void Add(WishList entity)
    {
        WishLists.Add(entity);
    }

    public void Remove(WishList entity)
    {
        WishLists.Remove(entity);
    }

    public void Update(WishList entity)
    {
        Remove(entity);
        Add(entity);
    }

    public void AddWish(WishList entity, Wish wish)
    {
        // That's in memory storage, so entity already contains wish
    }
    
    public void RemoveWish(WishList entity, Wish wish)
    {
        // That's in memory storage, so entity already does not contain wish
    }

    public Task<WishList?> GetByIdAsync(Guid wishListId, CancellationToken cancellationToken)
    {
        return Task.FromResult(WishLists.SingleOrDefault(wishList => wishList.Id == wishListId));
    }

    public Task<WishList?> GetByIdWithoutWishesAsync(Guid wishListId, CancellationToken cancellationToken)
    {
        return Task.FromResult(WishLists.SingleOrDefault(wishList => wishList.Id == wishListId));
    }

    public Task<bool> HasUserAccessAsync(WishList wishList, User user, CancellationToken cancellationToken)
    {
        if (!WishListUserAccess.ContainsKey(wishList))
        {
            WishListUserAccess.Add(wishList, []);
        }
        
        return Task.FromResult(WishListUserAccess[wishList].Contains(user));
    }

    public void AllowUserAccess(WishList wishList, User user)
    {
        if (HasUserAccessAsync(wishList, user, CancellationToken.None).GetAwaiter().GetResult())
        {
            return;
        }
        
        WishListUserAccess[wishList].Add(user);
    }

    public void DenyUserAccess(WishList wishList, User user)
    {
        if (!HasUserAccessAsync(wishList, user, CancellationToken.None).GetAwaiter().GetResult())
        {
            return;
        }
        
        WishListUserAccess[wishList].Remove(user);
    }

    public Task<List<WishList>> GetWithOwnerAsync(User owner, CancellationToken cancellationToken)
    {
        return Task.FromResult(WishLists.Where(wishList => wishList.Owner == owner).ToList());
    }
    
    public Task<List<WishList>> GetWithOwnerAndUserAccessAsync(User owner, User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(WishLists.Where(wishList =>
            wishList.Owner == owner && HasUserAccessAsync(wishList, user, cancellationToken).Result).ToList());
    }

    public Task<bool> ExistsContainingWishWithUserAccessAsync(Wish wish, User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(WishLists.Any(wishList =>
            wishList.Wishes.Contains(wish) && HasUserAccessAsync(wishList, user, cancellationToken).Result));
    }
}