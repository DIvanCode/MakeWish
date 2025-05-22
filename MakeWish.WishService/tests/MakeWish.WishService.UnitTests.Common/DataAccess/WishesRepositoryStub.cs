using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class WishesRepositoryStub(GlobalStorage globalStorage) : IWishesRepository
{
    private List<Wish> Wishes => globalStorage.Wishes;

    public void Add(Wish entity)
    {
        Wishes.Add(entity);
    }

    public void Remove(Wish entity)
    {
        Wishes.Remove(entity);
    }

    public void Update(Wish entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Wishes.SingleOrDefault(e => e.Id == id));
    }

    public Task<List<Wish>> GetWithStatusAndOwnerAsync(WishStatus status, User owner, CancellationToken cancellationToken)
    {
        return Task.FromResult(Wishes.Where(wish => wish.GetStatusFor(owner) == status && wish.Owner == owner).ToList());
    }
    
    public Task<List<Wish>> GetWithOwnerAsync(User owner, CancellationToken cancellationToken)
    {
        return Task.FromResult(Wishes.Where(wish => wish.Owner == owner).ToList());
    }

    public Task<List<Wish>> GetWithPromiserAsync(User promiser, CancellationToken cancellationToken)
    {
        return Task.FromResult(Wishes.Where(wish => wish.GetPromiserFor(promiser) == promiser).ToList());
    }
}