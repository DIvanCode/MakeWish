using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Repositories;

public sealed class FriendshipsRepository(DbSet<Friendship> entities)
    : BaseRepository<Friendship>(entities), IFriendshipsRepository
{
    public async Task<Friendship?> GetBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return await entities.SingleOrDefaultAsync(e =>
                (e.FirstUser.Id == firstUser.Id && e.SecondUser.Id == secondUser.Id) ||
                (e.FirstUser.Id == secondUser.Id && e.SecondUser.Id == firstUser.Id),
            cancellationToken);
    }

    public async Task<Friendship?> GetByUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return await entities.SingleOrDefaultAsync(e =>
                e.FirstUser.Id == firstUser.Id && e.SecondUser.Id == secondUser.Id,
            cancellationToken);
    }

    public async Task<bool> HasBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return await entities.AnyAsync(e =>
                (e.FirstUser.Id == firstUser.Id && e.SecondUser.Id == secondUser.Id) ||
                (e.FirstUser.Id == secondUser.Id && e.SecondUser.Id == firstUser.Id),
            cancellationToken);
    }

    public async Task<List<Friendship>> GetAllConfirmedAsync(CancellationToken cancellationToken)
    {
        return await entities
            .Where(e => e.IsConfirmed)
            .Include(e => e.FirstUser)
            .Include(e => e.SecondUser)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Friendship>> GetConfirmedForUserAsync(User user, CancellationToken cancellationToken)
    {
        return await entities
            .Where(e => e.IsConfirmed && (e.FirstUser.Id == user.Id || e.SecondUser.Id == user.Id))
            .Include(e => e.FirstUser)
            .Include(e => e.SecondUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Friendship>> GetPendingFromUserAsync(User user, CancellationToken cancellationToken)
    {
        return await entities
            .Where(e => !e.IsConfirmed && e.FirstUser.Id == user.Id)
            .Include(e => e.SecondUser)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Friendship>> GetPendingToUserAsync(User user, CancellationToken cancellationToken)
    {
        return await entities
            .Where(e => !e.IsConfirmed && e.SecondUser.Id == user.Id)
            .Include(e => e.FirstUser)
            .ToListAsync(cancellationToken);
    }
}