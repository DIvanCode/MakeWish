using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IWishesRepository : IBaseRepository<Wish>
{
    Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Wish>> GetWithPromiserAsync(User promiser, CancellationToken cancellationToken);
    Task<List<Wish>> GetWithStatusAndOwnerAsync(WishStatus status, User owner, CancellationToken cancellationToken);
    Task<List<Wish>> GetWithOwnerAsync(User owner, CancellationToken cancellationToken);
}