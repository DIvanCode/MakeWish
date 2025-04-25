using FluentResults;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Domain;

namespace MakeWish.Desktop.Clients.UserService;

public interface IUserServiceClient
{
    // Users
    Task<Result<User>> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result<string>> AuthenticateUserAsync(AuthenticateRequest request, CancellationToken cancellationToken);
    Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken);
    Task<Result<User>> GetCurrentUserAsync(CancellationToken cancellationToken);
    Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken);
    
    // Friendships
    Task<Result<Friendship>> CreateFriendshipAsync(CreateFriendshipRequest request, CancellationToken cancellationToken);
    Task<Result<Friendship>> ConfirmFriendshipAsync(ConfirmFriendshipRequest request, CancellationToken cancellationToken);
    Task<Result> RemoveFriendshipAsync(RemoveFriendshipRequest request, CancellationToken cancellationToken);
    Task<Result<List<Friendship>>> GetConfirmedFriendshipsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<List<Friendship>>> GetPendingFriendshipsFromUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<List<Friendship>>> GetPendingFriendshipsToUserAsync(Guid userId, CancellationToken cancellationToken);
}