using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UseCases.Features.Users.Create;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Users.Create;

public class CreateUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _handler = new CreateUserHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenUserDoesNotExist()
    {
        // Arrange
        const string name = "John";
        const string surname = "Doe";
        
        var userId = Guid.NewGuid();
        
        var command = new CreateUserCommand(userId, name, surname);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
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
        
        var command = new CreateUserCommand(userId, name, surname);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityAlreadyExistsError);
    }
}