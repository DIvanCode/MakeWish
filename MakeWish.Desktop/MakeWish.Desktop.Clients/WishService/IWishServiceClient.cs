using FluentResults;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Domain;

namespace MakeWish.Desktop.Clients.WishService;

public interface IWishServiceClient
{
    // Wishes
    Task<Result<Wish>> GetWishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<Wish>>> SearchWishAsync(Guid userId, string query, CancellationToken cancellationToken);
    Task<Result<Wish>> CreateWishAsync(CreateWishRequest request, CancellationToken cancellationToken);
    Task<Result<Wish>> UpdateWishAsync(UpdateWishRequest request, CancellationToken cancellationToken);
    Task<Result<Wish>> DeleteWishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> RestoreWishAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<List<Wish>>> GetUserWishesAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<List<Wish>>> GetPromisedWishesAsync(CancellationToken cancellationToken);
    Task<Result<Wish>> PromiseAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> PromiseCancelAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> CompleteAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> CompleteApproveAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<Wish>> CompleteRejectAsync(Guid id, CancellationToken cancellationToken);
    
    // WishLists
    Task<Result<WishList>> GetWishListAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<WishList>> CreateWishListAsync(CreateWishListRequest request, CancellationToken cancellationToken);
    Task<Result<WishList>> UpdateWishListAsync(UpdateWishListRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteWishListAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<WishList>> AddWishToWishListAsync(Guid id, Guid wishId, CancellationToken cancellationToken);
    Task<Result<WishList>> RemoveWishFromWishListAsync(Guid id, Guid wishId, CancellationToken cancellationToken);
    Task<Result<List<WishList>>> GetUserWishListsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> AllowUserAccessToWishListAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Result> DenyUserAccessToWishListAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<Result<List<User>>> GetUsersWithAccessToWishListAsync(Guid wishListId, CancellationToken cancellationToken);
}