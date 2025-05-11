using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IWishListsRepository : IBaseRepository<WishList>
{
    void AddWish(WishList wishList, Wish wish);
    void RemoveWish(WishList wishList, Wish wish);
    Task<WishList?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<WishList?> GetByIdWithoutWishesAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> HasUserAccessAsync(WishList wishList, User user, CancellationToken cancellationToken);
    Task<bool> ExistsContainingWishWithUserAccessAsync(Wish wish, User user, CancellationToken cancellationToken);
    void AllowUserAccess(WishList wishList, User user);
    void DenyUserAccess(WishList wishList, User user);
    Task<List<WishList>> GetWithOwnerAsync(User owner, CancellationToken cancellationToken);
    Task<List<WishList>> GetWithOwnerAndUserAccessAsync(User owner, User user, CancellationToken cancellationToken);
}