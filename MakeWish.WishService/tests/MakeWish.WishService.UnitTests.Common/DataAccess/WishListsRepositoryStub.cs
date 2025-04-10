using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class WishListsRepositoryStub : IWishListsRepository
{
    private readonly List<WishList> _wishLists = [];
    private readonly Dictionary<WishList, List<User>> _allowedAccessors = new();

    public void Add(WishList entity)
    {
        _wishLists.Add(entity);
    }

    public void Remove(WishList entity)
    {
        _wishLists.Remove(entity);
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
        return Task.FromResult(_wishLists.SingleOrDefault(wishList => wishList.Id == wishListId));
    }

    public Task<bool> HasUserAccessAsync(WishList wishList, User user, CancellationToken cancellationToken)
    {
        if (!_allowedAccessors.ContainsKey(wishList))
        {
            _allowedAccessors.Add(wishList, []);
        }
        
        return Task.FromResult(_allowedAccessors[wishList].Contains(user));
    }

    public void AllowUserAccess(WishList wishList, User user, CancellationToken cancellationToken)
    {
        if (HasUserAccessAsync(wishList, user, cancellationToken).Result)
        {
            return;
        }
        
        _allowedAccessors[wishList].Add(user);
    }

    public void DenyUserAccess(WishList wishList, User user, CancellationToken cancellationToken)
    {
        if (!HasUserAccessAsync(wishList, user, cancellationToken).Result)
        {
            return;
        }
        
        _allowedAccessors[wishList].Remove(user);
    }

    public Task<List<WishList>> GetWishListsWithOwnerAsync(User owner, CancellationToken cancellationToken)
    {
        return Task.FromResult(_wishLists.Where(wishList => wishList.Owner == owner).ToList());
    }

    public Task<bool> ExistsContainingWishWithUserAccessAsync(Wish wish, User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(_wishLists.Any(wishList =>
            wishList.Wishes.Contains(wish) && HasUserAccessAsync(wishList, user, cancellationToken).Result));
    }

    public Task<List<WishList>> GetWishListsWithUserAccessAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(_wishLists.Where(wishList => HasUserAccessAsync(wishList, user, cancellationToken).Result).ToList());
    }
}