using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.UnitTests.Common.Models;

public sealed class FriendshipBuilder
{
    private User _firstUser = new UserBuilder().Build();
    private User _secondUser = new UserBuilder().Build();
    
    public FriendshipBuilder WithFirstUser(User user)
    {
        _firstUser = user;
        return this;
    }
    
    public FriendshipBuilder WithSecondUser(User user)
    {
        _secondUser = user;
        return this;
    }

    public Friendship Build()
    {
        return Friendship.Create(_firstUser, _secondUser).Value;
    }
}