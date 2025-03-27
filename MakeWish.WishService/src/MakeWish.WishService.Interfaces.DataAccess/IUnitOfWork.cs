namespace MakeWish.WishService.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IWishesRepository Wishes { get; }
    IUsersRepository Users { get; }
    IWishListsRepository WishLists { get; }
    // IWishListUserPermissionsRepository WishListUserPermissions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}