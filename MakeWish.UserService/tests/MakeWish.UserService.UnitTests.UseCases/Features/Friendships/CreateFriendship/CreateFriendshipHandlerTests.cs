using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.DataAccess;
using MakeWish.UserService.UseCases.Features.Friendships.CreateFriendship;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.CreateFriendship;

public class CreateFriendshipHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateFriendshipHandler _sut;

    public CreateFriendshipHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new CreateFriendshipHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new CreateFriendshipCommand(Guid.NewGuid(), Guid.NewGuid());

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
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new CreateFriendshipCommand(Guid.NewGuid(), Guid.NewGuid());

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
        var firstUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(firstUser);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);
        
        var command = new CreateFriendshipCommand(firstUser.Id, Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_FriendshipAlreadyExists_ReturnsEntityAlreadyExistsError()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);
        
        var friendship = Friendship.Create(firstUser, secondUser);
        
        _unitOfWork.Friendships.Add(friendship);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);
        
        var command = new CreateFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityAlreadyExistsError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulFriendshipCreation_ReturnsFriendshipDto()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);
        
        var command = new CreateFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(firstUser.Id, result.Value.FirstUserId);
        Assert.Equal(secondUser.Id, result.Value.SecondUserId);
        Assert.False(result.Value.IsConfirmed);
    }
}
