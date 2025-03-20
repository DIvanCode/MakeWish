using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;

namespace MakeWish.UserService.Adapters.DataAccess.InMemory;

public sealed class FriendshipsRepositoryStub : IFriendshipsRepository
{
    private readonly List<Friendship> _friendships = [];
    
    public void Add(Friendship entity)
    {
        _friendships.Add(entity);
    }

    public void Remove(Friendship entity)
    {
        _friendships.Remove(entity);
    }

    public void Update(Friendship entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<Friendship?> GetBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .SingleOrDefault(e =>
                (e.FirstUser.Id == firstUser.Id && e.SecondUser.Id == secondUser.Id) ||
                (e.FirstUser.Id == secondUser.Id && e.SecondUser.Id == firstUser.Id)));
    }

    public Task<Friendship?> GetByUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .SingleOrDefault(e =>
                e.FirstUser.Id == firstUser.Id &&
                e.SecondUser.Id == secondUser.Id));
    }

    public Task<bool> HasBetweenUsersAsync(User firstUser, User secondUser, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .Any(e =>
                (e.FirstUser.Id == firstUser.Id && e.SecondUser.Id == secondUser.Id) ||
                (e.FirstUser.Id == secondUser.Id && e.SecondUser.Id == firstUser.Id)));
    }

    public Task<List<Friendship>> GetConfirmedForUserAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .Where(e => 
                e.IsConfirmed && (e.FirstUser.Id == user.Id || e.SecondUser.Id == user.Id))
            .ToList());
    }

    public Task<List<Friendship>> GetPendingFromUserAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .Where(e => !e.IsConfirmed && e.FirstUser.Id == user.Id)
            .ToList());
    }

    public Task<List<Friendship>> GetPendingToUserAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(_friendships
            .Where(e => !e.IsConfirmed && e.SecondUser.Id == user.Id)
            .ToList());
    }
}