using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.Utils.Errors;

namespace MakeWish.UserService.UnitTests.Models;

public class FriendshipTests
{
    [Fact]
    public void Create_ValidUsers_ShouldCreateFriendship()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();

        // Act
        var result = Friendship.Create(user1, user2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotEqual(user1, user2);
        Assert.Equal(user1, result.Value.FirstUser);
        Assert.Equal(user2, result.Value.SecondUser);
        Assert.False(result.Value.IsConfirmed);
    }

    [Fact]
    public void Create_SameUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act
        var result = Friendship.Create(user, user);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<BadRequestError>(result.Errors.First());
    }

    [Fact]
    public void ConfirmBy_ValidUser_ShouldConfirmFriendship()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = Friendship.Create(user1, user2).Value;

        // Act
        var result = friendship.ConfirmBy(user2);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(friendship.IsConfirmed);
    }

    [Fact]
    public void ConfirmBy_InvalidUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var invalidUser = new UserBuilder().Build();
        var friendship = Friendship.Create(user1, user2).Value;

        // Act
        var result = friendship.ConfirmBy(invalidUser);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<ForbiddenError>(result.Errors.First());
    }
}