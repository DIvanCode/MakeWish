using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Features.Friendships.RemoveFriendship;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.DataAccess;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.RemoveFriendship;

public class RemoveFriendshipHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RemoveFriendshipHandler _sut;

    public RemoveFriendshipHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new RemoveFriendshipHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new RemoveFriendshipCommand(Guid.NewGuid(), Guid.NewGuid());

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
        
        var command = new RemoveFriendshipCommand(Guid.NewGuid(), Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<ForbiddenError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_FirstUserNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(secondUser);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);
        
        var command = new RemoveFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }
    
    [Fact]
    public async Task Handle_SecondUserNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(firstUser);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);
        
        var command = new RemoveFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_FriendshipNotFound_ReturnsEntityNotFoundError()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);

        var command = new RemoveFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulFriendshipRemovalByFirstUser_ReturnsOk()
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

        var command = new RemoveFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task Handle_SuccessfulFriendshipRemovalBySecondUser_ReturnsOk()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        var friendship = Friendship.Create(firstUser, secondUser);
        
        _unitOfWork.Friendships.Add(friendship);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(secondUser.Id);

        var command = new RemoveFriendshipCommand(firstUser.Id, secondUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }
}
