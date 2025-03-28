using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Users.Delete;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Users.Delete;

public class DeleteUserHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _handler = new DeleteUserHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(user);
        
        var command = new DeleteUserCommand(user.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var deletedUser = await _unitOfWork.Users.GetByIdAsync(user.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new DeleteUserCommand(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
}