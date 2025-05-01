using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.Models.Events;

namespace MakeWish.UserService.UnitTests.Models;

public class UserTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateUser()
    {
        // Arrange
        const string email = "test@example.com";
        const string passwordHash = "hashedpassword";
        const string name = "John";
        const string surname = "Doe";
        
        // Act
        var user = User.Create(email, passwordHash, name, surname);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(name, user.Name);
        Assert.Equal(surname, user.Surname);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidEmail_ShouldThrowArgumentException(string invalidEmail)
    {
        // Arrange
        const string passwordHash = "hashedpassword";
        const string name = "John";
        const string surname = "Doe";

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => User.Create(invalidEmail, passwordHash, name, surname));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidPasswordHash_ShouldThrowArgumentException(string invalidPasswordHash)
    {
        // Arrange
        const string email = "test@example.com";
        const string name = "John";
        const string surname = "Doe";

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => User.Create(email, invalidPasswordHash, name, surname));
    }

    [Fact]
    public void Create_ShouldCreateDomainEvent()
    {
        // Arrange
        const string email = "test@example.com";
        const string passwordHash = "hashedpassword";
        const string name = "John";
        const string surname = "Doe";
        
        // Act
        var user = User.Create(email, passwordHash, name, surname);

        // Assert
        var domainEvents = user.CollectDomainEvents();
        Assert.Single(domainEvents);
        Assert.IsType<UserCreatedEvent>(domainEvents[0]);
        
        var @event = (UserCreatedEvent)domainEvents[0];
        Assert.Equivalent(user, @event.User);
    }
    
    [Fact]
    public void Delete_ShouldCreateDomainEvent()
    {
        // Arrange
        const string email = "test@example.com";
        const string passwordHash = "hashedpassword";
        const string name = "John";
        const string surname = "Doe";
        
        // Act
        var user = User.Create(email, passwordHash, name, surname);
        user.CollectDomainEvents();
        user.Delete();

        // Assert
        var domainEvents = user.CollectDomainEvents();
        Assert.Single(domainEvents);
        Assert.IsType<UserDeletedEvent>(domainEvents[0]);
        
        var @event = (UserDeletedEvent)domainEvents[0];
        Assert.Equivalent(user, @event.User);
    }
}