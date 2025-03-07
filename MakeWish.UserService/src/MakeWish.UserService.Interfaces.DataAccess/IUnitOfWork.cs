namespace MakeWish.UserService.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IFriendshipsRepository Friendships { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}