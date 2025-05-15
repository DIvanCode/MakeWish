using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.Interfaces.DataAccess;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<User>> GetBySearchQueryAsync(string searchQuery, CancellationToken cancellationToken);
    Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    Task<bool> HasWithEmailAsync(string email, CancellationToken cancellationToken);
}