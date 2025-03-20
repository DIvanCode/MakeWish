using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Users.Delete;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.Delete;

public class DeleteHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteHandler _sut;

    public DeleteHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new DeleteHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new DeleteCommand(userId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<AuthenticationError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_UserNotAuthorized_ReturnsForbiddenError()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var userIdToDelete = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(currentUserId);
        
        var command = new DeleteCommand(userIdToDelete);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<ForbiddenError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(userId);
        
        var command = new DeleteCommand(userId);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfullyDeleted_ReturnsSuccess()
    {
        // Arrange
        var user = new UserBuilder().Build(); 
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        _unitOfWork.Users.Add(user);

        var command = new DeleteCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);
        var shouldBeDeletedUser = await _unitOfWork.Users.GetByIdAsync(user.Id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Null(shouldBeDeletedUser);
    }
}
