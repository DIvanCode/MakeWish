using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class UsersRepositoryStub : IUsersRepository
{
    private readonly List<User> _users = [];

    public void Add(User entity)
    {
        _users.Add(entity);
    }

    public void Remove(User entity)
    {
        _users.Remove(entity);
    }

    public void Update(User entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.SingleOrDefault(e => e.Id == id));
    }
}