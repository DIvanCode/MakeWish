using FluentResults;
using MakeWish.UserService.Utils.Errors;

namespace MakeWish.UserService.Models;

public sealed class Friendship
{
    public User FirstUser { get; init; }

    public User SecondUser { get; init; }
    
    public bool IsConfirmed { get; private set; }
    
    // Do not call explicitly: only for EF
    // ReSharper disable once UnusedMember.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Friendship() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    
    private Friendship(User firstUser, User secondUser)
    {
        FirstUser = firstUser;
        SecondUser = secondUser;
    }

    public static Result<Friendship> Create(User firstUser, User secondUser)
    {
        if (firstUser.Id == secondUser.Id)
        {
            return new BadRequestError("Cannot create friendship with yourself.");
        }

        return new Friendship(firstUser, secondUser);
    }

    public Result ConfirmBy(User confirmUser)
    {
        if (confirmUser.Id != SecondUser.Id)
        {
            return new ForbiddenError(
                nameof(Friendship),
                "confirm",
                nameof(SecondUser.Id),
                SecondUser.Id);
        }
        
        IsConfirmed = true;
        return Result.Ok();
    }
}