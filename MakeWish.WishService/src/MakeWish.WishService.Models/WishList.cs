using EnsureThat;
using FluentResults;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.Models;

public sealed class WishList
{
    public Guid Id { get; init; }
    public string Title { get; private set; }
    public User Owner { get; init; }
    public IReadOnlyList<Wish> Wishes => _wishes.AsReadOnly();
    
    private readonly List<Wish> _wishes;

    private WishList(Guid id, string title, User owner)
    {
        Id = id;
        Title = title;
        Owner = owner;
        _wishes = [];
    }

    public static WishList Create(string title, User owner)
    {
        EnsureArg.IsNotNullOrWhiteSpace(title);
        EnsureArg.IsNotNull(owner);
        
        var id = Guid.NewGuid();
        return new WishList(id, title, owner);
    }

    public Result Update(string title, User by)
    {
        EnsureArg.IsNotNullOrWhiteSpace(title);
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(WishList), nameof(Update), nameof(Owner), by.Id);
        }
        
        Title = title;
        return Result.Ok();
    }
    
    public Result Add(Wish wish, User by)
    {
        EnsureArg.IsNotNull(wish);
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(WishList), nameof(Add), nameof(Id), Id);
        }

        if (Contains(wish))
        {
            return new EntityAlreadyExistsError(nameof(WishList), nameof(Wish), nameof(Wish.Id), wish.Id);
        }
        
        _wishes.Add(wish);
        return Result.Ok();
    }

    public Result Remove(Wish wish, User by)
    {
        EnsureArg.IsNotNull(wish);
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(WishList), nameof(Remove), nameof(Id), Id);
        }
        
        if (!Contains(wish))
        {
            return new EntityNotFoundError(nameof(WishList), nameof(Wish), nameof(Wish.Id), wish.Id);
        }
        
        _wishes.RemoveAll(e => e.Id == wish.Id);
        return Result.Ok();
    }

    public bool CanUserManageAccess(User user)
    {
        return user.Id == Owner.Id;
    }

    private bool Contains(Wish wish)
    {
        return _wishes.Any(e => e.Id == wish.Id);
    }
}