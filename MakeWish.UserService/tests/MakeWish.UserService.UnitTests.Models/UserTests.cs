using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;

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
        var builder = new UserBuilder().WithEmail(invalidEmail);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => builder.Build());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_InvalidPasswordHash_ShouldThrowArgumentException(string invalidPasswordHash)
    {
        // Arrange
        var builder = new UserBuilder().WithPasswordHash(invalidPasswordHash);

        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => builder.Build());
    }
}