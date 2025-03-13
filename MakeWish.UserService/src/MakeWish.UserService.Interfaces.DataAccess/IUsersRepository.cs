using MakeWish.UserService.Models;

namespace MakeWish.UserService.Interfaces.DataAccess;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> HasWithEmailAsync(string email, CancellationToken cancellationToken);
}