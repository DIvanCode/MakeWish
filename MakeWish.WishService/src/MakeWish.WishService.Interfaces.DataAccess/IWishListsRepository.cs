using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IWishListsRepository : IBaseRepository<WishList>
{
    void AddWish(WishList wishList, Wish wish);
    void RemoveWish(WishList wishList, Wish wish);
    Task<WishList?> GetByIdAsync(Guid wishListId, CancellationToken cancellationToken);
    Task<bool> HasUserAccessAsync(WishList wishList, User user, CancellationToken cancellationToken);
    Task<bool> ExistsContainingWishWithUserAccessAsync(Wish wish, User user, CancellationToken cancellationToken);
    void AllowUserAccess(WishList wishList, User user, CancellationToken cancellationToken);
    void DenyUserAccess(WishList wishList, User user, CancellationToken cancellationToken);
    Task<List<WishList>> GetWishListsWithOwnerAsync(User owner, CancellationToken cancellationToken);
    Task<List<WishList>> GetWishListsWithUserAccessAsync(User user, CancellationToken cancellationToken);
}