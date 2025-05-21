using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class UsersRepositoryStub(GlobalStorage globalStorage) : IUsersRepository
{
    private List<User> Users => globalStorage.Users;
    private Dictionary<WishList, List<User>> WishListUserAccess => globalStorage.WishListUserAccess;
    
    public void Add(User entity)
    {
        Users.Add(entity);
    }

    public void Remove(User entity)
    {
        Users.Remove(entity);
    }

    public void Update(User entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Users.SingleOrDefault(e => e.Id == id));
    }

    public Task<List<User>> GetUsersWithAccessToWishListAsync(WishList wishList, CancellationToken cancellationToken)
    {
        return Task.FromResult(Users.Where(user => WishListUserAccess[wishList].Contains(user)).ToList());
    }
}