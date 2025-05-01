using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.Models.Events;
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
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

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
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        var result = friendship.ConfirmBy(invalidUser);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<ForbiddenError>(result.Errors.First());
    }
    
    [Fact]
    public void RemoveBy_ValidUser1_ShouldRemoveFriendship()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        var result = friendship.RemoveBy(user1);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void RemoveBy_ValidUser2_ShouldRemoveFriendship()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        var result = friendship.RemoveBy(user2);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void RemoveBy_InvalidUser_ShouldThrowArgumentException()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var invalidUser = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        var result = friendship.RemoveBy(invalidUser);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<ForbiddenError>(result.Errors.First());
    }
    
    [Fact]
    public void ConfirmBy_ShouldCreateDomainEvent()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        friendship.ConfirmBy(user2);

        // Assert
        var domainEvents = friendship.CollectDomainEvents();
        Assert.Single(domainEvents);
        Assert.IsType<FriendshipConfirmedEvent>(domainEvents[0]);
        
        var @event = (FriendshipConfirmedEvent)domainEvents[0];
        Assert.Equivalent(friendship, @event.Friendship);
    }
    
    [Fact]
    public void RemoveBy_NotConfirmedFriendship_ShouldNotCreateDomainEvent()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        friendship.RemoveBy(user2);

        // Assert
        var domainEvents = friendship.CollectDomainEvents();
        Assert.Empty(domainEvents);
    }
    
    [Fact]
    public void RemoveBy_ConfirmedFriendship_ShouldCreateDomainEvent()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var friendship = new FriendshipBuilder().WithFirstUser(user1).WithSecondUser(user2).Build();

        // Act
        friendship.ConfirmBy(user2);
        friendship.CollectDomainEvents();
        friendship.RemoveBy(user2);

        // Assert
        var domainEvents = friendship.CollectDomainEvents();
        Assert.Single(domainEvents);
        Assert.IsType<FriendshipRemovedEvent>(domainEvents[0]);
        
        var @event = (FriendshipRemovedEvent)domainEvents[0];
        Assert.Equivalent(friendship, @event.Friendship);
    }
}