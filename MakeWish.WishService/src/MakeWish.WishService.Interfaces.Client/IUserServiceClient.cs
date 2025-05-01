using FluentResults;
using MakeWish.WishService.Interfaces.Client.Responses;

namespace MakeWish.WishService.Interfaces.Client;

public interface IUserServiceClient
{
    Task<Result<List<UserResponse>>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<Result<List<FriendshipResponse>>> GetAllFriendshipsAsync(CancellationToken cancellationToken);
}