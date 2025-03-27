using MakeWish.UserService.Interfaces.DataAccess;

namespace MakeWish.UserService.Adapters.DataAccess.InMemory;

public sealed class UnitOfWorkStub : IUnitOfWork
{
    public IUsersRepository Users => _usersRepository.Value;
    public IFriendshipsRepository Friendships => _friendshipsRepository.Value;
    
    private readonly Lazy<IUsersRepository> _usersRepository =
        new(() => new UsersRepositoryStub());
    private readonly Lazy<IFriendshipsRepository> _friendshipsRepository =
        new(() => new FriendshipsRepositoryStub());

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1); 
    }
}