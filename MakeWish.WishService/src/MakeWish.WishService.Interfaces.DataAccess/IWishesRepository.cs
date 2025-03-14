using MakeWish.WishService.Models;

namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IWishesRepository : IBaseRepository<Wish>
{
    Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}