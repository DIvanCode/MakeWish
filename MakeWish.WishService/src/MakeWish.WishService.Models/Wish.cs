using EnsureThat;
using FluentResults;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.Models;

public sealed class Wish : Entity
{
    public string Title { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;
    public User Owner { get; } = default!;

    private WishStatus _status;
    private User? _promiser;
    private User? _completer;
    
    // ReSharper disable once UnusedMember.Local
    public Wish()
    {
    }

    private Wish(Guid id, string title, string description, User owner, WishStatus status)
    {
        Id = id;
        Title = title;
        Description = description;
        Owner = owner;
        _status = status;
    }

    public WishStatus GetStatusFor(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        if (_status is WishStatus.Promised)
        {
            EnsureArg.IsNotNull(_promiser, nameof(_promiser));
            
            if (user.Id == Owner.Id && _promiser.Id != Owner.Id)
            {
                return WishStatus.Created;
            }

            return WishStatus.Promised;
        }

        if (_status is WishStatus.Completed)
        {
            EnsureArg.IsNotNull(_completer, nameof(_completer));

            if (user.Id == Owner.Id || user.Id == _completer.Id)
            {
                return WishStatus.Completed;
            }

            return WishStatus.Promised;
        }

        if (_status is WishStatus.Approved)
        {
            EnsureArg.IsNotNull(_completer, nameof(_completer));

            if (user.Id == Owner.Id || user.Id == _completer.Id)
            {
                return WishStatus.Approved;
            }

            return WishStatus.Completed;
        }

        return _status;
    }
    
    public User? GetPromiserFor(User user)
    {
        EnsureArg.IsNotNull(user, nameof(user));

        if (_status is not WishStatus.Promised || user.Id != Owner.Id)
        {
            return _promiser;
        }

        EnsureArg.IsNotNull(_promiser, nameof(_promiser));
        return Owner.Id == _promiser.Id ? _promiser : null;
    }
    
    public User? GetCompleter()
    {
        return _completer;
    }
    
    public static Wish Create(string title, string? description, User owner)
    {
        EnsureArg.IsNotEmptyOrWhiteSpace(title, nameof(title));
        EnsureArg.IsNotNull(owner, nameof(owner));

        var id = Guid.NewGuid();
        return new Wish(id, title, description ?? string.Empty, owner, WishStatus.Created);
    }

    public Result Update(string title, string description, User by)
    {
        EnsureArg.IsNotEmptyOrWhiteSpace(title, nameof(title));
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(Update), nameof(Id), Id);
        }
        
        if (_status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), nameof(Update), nameof(_status), _status);
        }

        Title = title;
        Description = description;

        return Result.Ok();
    }

    public Result Promise(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (_status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), nameof(Promise), nameof(_status), _status);
        }
        
        _promiser = by;
        _status = WishStatus.Promised;

        return Result.Ok();
    }
    
    public Result PromiseCancel(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (_status != WishStatus.Promised)
        {
            return new ForbiddenError(nameof(Wish), nameof(PromiseCancel), nameof(_status), _status);
        }
        
        EnsureArg.IsNotNull(_promiser, nameof(_promiser));
        if (_promiser.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(PromiseCancel), nameof(Id), Id);
        }
        
        _promiser = null;
        _status = WishStatus.Created;

        return Result.Ok();
    }
    
    public Result Complete(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (Owner.Id == by.Id)
        {
            switch (_status)
            {
                case WishStatus.Completed or WishStatus.Approved or WishStatus.Deleted:
                {
                    return new ForbiddenError(nameof(Wish), nameof(Complete), nameof(_status), _status);
                }
                case WishStatus.Promised:
                {
                    EnsureArg.IsNotNull(_promiser, nameof(_promiser));
                    if (_promiser.Id != by.Id)
                    {
                        return new ForbiddenError(nameof(Wish), nameof(Complete), nameof(Id), Id);
                    }
                    break;
                }
            }

            _completer = by;
            _status = WishStatus.Completed;
            
            var result = CompleteApprove(by);
            EnsureArg.IsTrue(result.IsSuccess);
        }
        else
        {
            if (_status != WishStatus.Promised)
            {
                return new ForbiddenError(nameof(Wish), nameof(Complete), nameof(_status), _status);
            }
            
            EnsureArg.IsNotNull(_promiser, nameof(_promiser));
            if (_promiser.Id != by.Id)
            {
                return new ForbiddenError(nameof(Wish), nameof(Complete), nameof(Id), Id);
            }

            _completer = by;
            _status = WishStatus.Completed;
        }

        return Result.Ok();
    }

    public Result CompleteApprove(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (_status != WishStatus.Completed)
        {
            return new ForbiddenError(nameof(Wish), nameof(CompleteApprove), nameof(_status), _status);
        }
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(CompleteApprove), nameof(Id), Id);
        }
        
        _status = WishStatus.Approved;

        return Result.Ok();
    }
    
    public Result CompleteReject(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (_status != WishStatus.Completed)
        {
            return new ForbiddenError(nameof(Wish), nameof(CompleteReject), nameof(_status), _status);
        }
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(CompleteReject), nameof(Id), Id);
        }
        
        _status = WishStatus.Created;
        _completer = null;
        _promiser = null;

        return Result.Ok();
    }
    
    public Result Delete(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(Delete), nameof(Id), Id);
        }

        if (_status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), nameof(Delete), nameof(_status), _status);
        }
        
        _status = WishStatus.Deleted;
        
        return Result.Ok();
    }
    
    public Result Restore(User by)
    {
        EnsureArg.IsNotNull(by, nameof(by));
        
        if (Owner.Id != by.Id)
        {
            return new ForbiddenError(nameof(Wish), nameof(Restore), nameof(Id), Id);
        }

        if (_status != WishStatus.Deleted)
        {
            return new ForbiddenError(nameof(Wish), nameof(Restore), nameof(_status), _status);
        }
        
        _status = WishStatus.Created;
        
        return Result.Ok();
    }

    public bool IsAccessible(User to, bool existsWishListContainingWishWithUserAccess)
    {
        EnsureArg.IsNotNull(to, nameof(to));
        
        return Owner.Id == to.Id || existsWishListContainingWishWithUserAccess;
    }
}