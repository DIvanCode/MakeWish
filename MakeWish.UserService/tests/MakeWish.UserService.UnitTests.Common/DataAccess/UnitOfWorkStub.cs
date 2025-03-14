using MakeWish.UserService.Interfaces.DataAccess;

namespace MakeWish.UserService.UnitTests.Common.DataAccess;

public sealed class UnitOfWorkStub : IUnitOfWork
{
    private readonly Lazy<IUsersRepository> _usersRepository =
        new(() => new UsersRepositoryStub());
    private readonly Lazy<IFriendshipsRepository> _friendshipsRepository =
        new(() => new FriendshipsRepositoryStub());

    public IUsersRepository Users => _usersRepository.Value;
    public IFriendshipsRepository Friendships => _friendshipsRepository.Value;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1); 
    }
}