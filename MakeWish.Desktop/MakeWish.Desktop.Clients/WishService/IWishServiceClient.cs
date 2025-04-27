using FluentResults;
using MakeWish.Desktop.Domain;

namespace MakeWish.Desktop.Clients.WishService;

public interface IWishServiceClient
{
    // Wishes
    Task<Result<Wish>> GetWishAsync(Guid id, CancellationToken cancellationToken);
    
    // WishLists
    Task<Result<WishList>> GetMainWishListAsync(CancellationToken cancellationToken);
    Task<Result<List<WishList>>> GetWishListsAsync(CancellationToken cancellationToken);
}