namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IWishesRepository Wishes { get; }
    IUsersRepository Users { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}