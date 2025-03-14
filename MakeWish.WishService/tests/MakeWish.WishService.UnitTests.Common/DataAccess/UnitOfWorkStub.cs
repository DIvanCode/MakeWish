using MakeWish.WishService.Interfaces.DataAccess;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class UnitOfWorkStub : IUnitOfWork
{
    private readonly Lazy<IUsersRepository> _usersRepository =
        new(() => new UsersRepositoryStub());
    private readonly Lazy<IWishesRepository> _wishesRepository =
        new(() => new WishesRepositoryStub());

    public IUsersRepository Users => _usersRepository.Value;
    public IWishesRepository Wishes => _wishesRepository.Value;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1); 
    }
}