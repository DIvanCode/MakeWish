using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Repositories;

public sealed class UsersRepository(DbSet<User> entities) : BaseRepository<User>(entities), IUsersRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await entities.SingleOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await entities.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<List<User>> GetBySearchQueryAsync(string searchQuery, CancellationToken cancellationToken)
    {
        return await entities
            .Where(u => u.Name + " " + u.Surname == searchQuery || u.Surname + " " + u.Name == searchQuery)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<bool> HasWithEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await entities.AnyAsync(e => e.Email == email, cancellationToken);
    }
}