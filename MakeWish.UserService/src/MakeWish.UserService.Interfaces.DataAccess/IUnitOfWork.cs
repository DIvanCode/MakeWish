namespace MakeWish.UserService.Interfaces.DataAccess;

public interface IUnitOfWork
{
    IUsersRepository Users { get; }
    IFriendshipsRepository Friendships { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}