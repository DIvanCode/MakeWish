using MakeWish.WishService.Interfaces.DataAccess;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class UnitOfWorkStub : IUnitOfWork
{
    private readonly GlobalStorage _globalStorage = new();
    private readonly Lazy<IUsersRepository> _usersRepository;
    private readonly Lazy<IWishesRepository> _wishesRepository;
    private readonly Lazy<IWishListsRepository> _wishListsRepository;

    public IUsersRepository Users => _usersRepository.Value;
    public IWishesRepository Wishes => _wishesRepository.Value;
    public IWishListsRepository WishLists => _wishListsRepository.Value;

    public UnitOfWorkStub()
    {
        _usersRepository = new Lazy<IUsersRepository>(() => new UsersRepositoryStub(_globalStorage));
        _wishesRepository = new Lazy<IWishesRepository>(() => new WishesRepositoryStub(_globalStorage));
        _wishListsRepository = new Lazy<IWishListsRepository>(() => new WishListsRepositoryStub(_globalStorage));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1); 
    }
}