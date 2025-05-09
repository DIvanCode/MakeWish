using FluentResults;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Domain;

namespace MakeWish.Desktop.Clients.WishService;

public interface IWishServiceClient
{
    // Wishes
    Task<Result<Wish>> GetWishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> CreateWishAsync(CreateWishRequest request, CancellationToken cancellationToken);
    Task<Result<Wish>> CompleteApproveAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> CompleteRejectAsync(Guid id, CancellationToken cancellationToken);
    
    // WishLists
    Task<Result<WishList>> GetMainWishListAsync(CancellationToken cancellationToken);
    Task<Result<WishList>> GetWishListAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<WishList>> CreateWishListAsync(CreateWishListRequest request, CancellationToken cancellationToken);
    Task<Result<List<WishList>>> GetWishListsAsync(CancellationToken cancellationToken);
}