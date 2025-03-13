using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;

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
        var friendship = Friendship.Create(user1, user2);

        // Assert
        Assert.NotNull(friendship);
        Assert.NotEqual(user1, user2);
        Assert.Equal(user1, friendship.FirstUser);
        Assert.Equal(user2, friendship.SecondUser);
        Assert.False(friendship.IsConfirmed);
    }

    [Fact]
    public void Create_SameUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user = new UserBuilder().Build();

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => Friendship.Create(user, user));
    }

    [Fact]
    public void ConfirmBy_ValidUser_ShouldConfirmFriendship()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = Friendship.Create(user1, user2);

        // Act
        friendship.ConfirmBy(user2);

        // Assert
        Assert.True(friendship.IsConfirmed);
    }

    [Fact]
    public void ConfirmBy_InvalidUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var invalidUser = new UserBuilder().Build();
        var friendship = Friendship.Create(user1, user2);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => friendship.ConfirmBy(invalidUser));
    }
}