using EnsureThat;
using FluentResults;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.Models;

public sealed class Wish
{
    public Guid Id { get; init; }

    public string Title { get; private set; }
    
    public string? Description { get; private set; }

    public WishStatus Status { get; private set; }

    public User Owner { get; init; }

    public User? Promiser { get; private set; }

    public User? Completer { get; private set; }

    private Wish(Guid id, string title, string? description, WishStatus status, User owner)
    {
        Id = id;
        Title = title;
        Description = description;
        Status = status;
        Owner = owner;
    }
    
    public static Wish Create(string title, string? description, User owner)
    {
        EnsureArg.IsNotEmptyOrWhiteSpace(title);

        var id = Guid.NewGuid();
        return new Wish(id, title, description, WishStatus.Created, owner);
    }

    public Result Update(string title, string? description, User updateBy)
    {
        EnsureArg.IsNotEmptyOrWhiteSpace(title);
        
        if (Owner.Id != updateBy.Id)
        {
            return new ForbiddenError(nameof(Wish), "update", nameof(Owner), updateBy.Id);
        }
        
        if (Status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), "update", nameof(Status), Status.ToString());
        }

        Title = title;
        Description = description;

        return Result.Ok();
    }

    public Result PromiseBy(User user)
    {
        if (Status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), "promise", nameof(Status), Status.ToString());
        }
        
        Promiser = user;
        Status = WishStatus.Promised;

        return Result.Ok();
    }
    
    public Result PromiseCancelBy(User user)
    {
        if (Status != WishStatus.Promised)
        {
            return new ForbiddenError(nameof(Wish), "promise cancel", nameof(Status), Status.ToString());
        }
        
        EnsureArg.IsNotNull(Promiser, nameof(Promiser));
        if (Promiser.Id != user.Id)
        {
            return new ForbiddenError(nameof(Wish), "promise cancel", nameof(Promiser), user.Id);
        }
        
        Promiser = null;
        Status = WishStatus.Created;

        return Result.Ok();
    }
    
    public Result CompleteBy(User user)
    {
        if (user.Id == Owner.Id)
        {
            switch (Status)
            {
                case WishStatus.Completed or WishStatus.Approved or WishStatus.Deleted:
                {
                    return new ForbiddenError(nameof(Wish), "complete", nameof(Status), Status.ToString());
                }
                case WishStatus.Promised:
                {
                    EnsureArg.IsNotNull(Promiser, nameof(Promiser));
                    if (Promiser.Id != user.Id)
                    {
                        return new ForbiddenError(nameof(Wish), "complete", nameof(Promiser), user.Id);
                    }
                    break;
                }
            }

            Completer = user;
            Status = WishStatus.Completed;
            
            var result = CompleteApproveBy(user);
            if (result.IsFailed)
            {
                return result;
            }
        }
        else
        {
            if (Status != WishStatus.Promised)
            {
                return new ForbiddenError(nameof(Wish), "complete", nameof(Status), Status.ToString());
            }
            
            EnsureArg.IsNotNull(Promiser, nameof(Promiser));
            if (Promiser.Id != user.Id)
            {
                return new ForbiddenError(nameof(Wish), "complete", nameof(Promiser), user.Id);
            }

            Completer = user;
            Status = WishStatus.Completed;
        }

        return Result.Ok();
    }

    public Result CompleteApproveBy(User user)
    {
        if (Status != WishStatus.Completed)
        {
            return new ForbiddenError(nameof(Wish), "complete approve", nameof(Status), Status.ToString());
        }
        
        if (user.Id != Owner.Id)
        {
            return new ForbiddenError(nameof(Wish), "complete approve", nameof(Id), Owner.Id);
        }
        
        Status = WishStatus.Approved;

        return Result.Ok();
    }
    
    public Result DeleteBy(User user)
    {
        if (user.Id != Owner.Id)
        {
            return new ForbiddenError(nameof(Wish), "delete", nameof(Id), Owner.Id);
        }

        if (Status != WishStatus.Created)
        {
            return new ForbiddenError(nameof(Wish), "delete", nameof(Status), Status.ToString());
        }
        
        Status = WishStatus.Deleted;
        
        return Result.Ok();
    }
    
    public Result RestoreBy(User user)
    {
        if (user.Id != Owner.Id)
        {
            return new ForbiddenError(nameof(Wish), "restore", nameof(Id), Owner.Id);
        }

        if (Status != WishStatus.Deleted)
        {
            return new ForbiddenError(nameof(Wish), "restore", nameof(Status), Status.ToString());
        }
        
        Status = WishStatus.Created;
        
        return Result.Ok();
    }
}