using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.Interfaces.DataAccess;

public interface IFriendshipsRepository : IBaseRepository<Friendship>
{
    Task<bool> HasBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken);
    Task<Friendship?> GetBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken);
    Task<Friendship?> GetByUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken);
    Task<List<Friendship>> GetConfirmedForUserAsync(User user, CancellationToken cancellationToken);
    Task<List<Friendship>> GetPendingFromUserAsync(User user, CancellationToken cancellationToken);
    Task<List<Friendship>> GetPendingToUserAsync(User user, CancellationToken cancellationToken);
}