using EnsureThat;

namespace MakeWish.UserService.Models;

public sealed class Friendship
{
    public User FirstUser { get; init; }

    public User SecondUser { get; init; }
    
    public bool IsConfirmed { get; set; }

    private Friendship(User firstUser, User secondUser)
    {
        FirstUser = firstUser;
        SecondUser = secondUser;
    }

    public static Friendship Create(User firstUser, User secondUser)
    {
        EnsureArg.IsNot(firstUser.Id, secondUser.Id, nameof(User.Id));

        return new Friendship(firstUser, secondUser);
    }

    public void ConfirmBy(User confirmUser)
    {
        EnsureArg.IsFalse(IsConfirmed, nameof(IsConfirmed));
        EnsureArg.Is(confirmUser.Id, SecondUser.Id, nameof(SecondUser.Id));
        
        IsConfirmed = true;
    }
}