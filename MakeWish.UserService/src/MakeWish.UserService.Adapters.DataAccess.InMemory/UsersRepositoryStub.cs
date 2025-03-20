using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;

namespace MakeWish.UserService.Adapters.DataAccess.InMemory;

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

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.SingleOrDefault(e => e.Email == email));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.SingleOrDefault(e => e.Id == id));
    }

    public Task<bool> HasWithEmailAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.Any(e => e.Email == email));
    }
}