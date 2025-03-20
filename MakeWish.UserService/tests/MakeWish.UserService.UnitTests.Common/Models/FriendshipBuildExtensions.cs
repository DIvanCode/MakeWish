using MakeWish.UserService.Models;

namespace MakeWish.UserService.UnitTests.Common.Models;

public static class FriendshipBuildExtensions
{
    public static Friendship ConfirmedBy(this Friendship friendship, User user)
    {
        friendship.ConfirmBy(user);
        return friendship;
    }
}