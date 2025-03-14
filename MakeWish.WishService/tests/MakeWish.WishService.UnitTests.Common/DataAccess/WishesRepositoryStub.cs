using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UnitTests.Common.DataAccess;

public sealed class WishesRepositoryStub : IWishesRepository
{
    private readonly List<Wish> _wishes = [];

    public void Add(Wish entity)
    {
        _wishes.Add(entity);
    }

    public void Remove(Wish entity)
    {
        _wishes.Remove(entity);
    }

    public void Update(Wish entity)
    {
        Remove(entity);
        Add(entity);
    }

    public Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_wishes.SingleOrDefault(e => e.Id == id));
    }
}