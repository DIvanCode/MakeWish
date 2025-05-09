using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.UnitTests.UseCases.Events;

public class UserCreatedHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserCreatedHandler _createdHandler;

    public UserCreatedHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _createdHandler = new UserCreatedHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        const string name = "John";
        const string surname = "Doe";
        
        var userId = Guid.NewGuid();
        
        var command = new UserCreatedNotification(userId, name, surname);
        
        // Act
        await _createdHandler.Handle(command, CancellationToken.None);

        // Assert
        var createdUser = await _unitOfWork.Users.GetByIdAsync(userId, CancellationToken.None);
        createdUser.Should().NotBeNull();
        createdUser.Name.Should().Be(name);
        createdUser.Surname.Should().Be(surname);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserAlreadyExists()
    {
        // Arrange
        const string name = "John";
        const string surname = "Doe";
        
        var userId = Guid.NewGuid();
        
        var existingUser = new User(userId, name, surname);
        _unitOfWork.Users.Add(existingUser);
        
        var command = new UserCreatedNotification(userId, name, surname);

        // Act & Assert
        await _createdHandler.Handle(command, CancellationToken.None);
    }
}