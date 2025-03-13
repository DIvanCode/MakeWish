using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.DataAccess;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsToUser;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.GetPendingFriendshipsToUser;

public class GetPendingFriendshipsToUserHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetPendingFriendshipsToUserHandler _sut;

    public GetPendingFriendshipsToUserHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetPendingFriendshipsToUserHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetPendingFriendshipsToUserCommand(Guid.NewGuid());

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
        
        var command = new GetPendingFriendshipsToUserCommand(Guid.NewGuid());

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
        var user = new UserBuilder().Build();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPendingFriendshipsToUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulReturnOfPendingFriendships_ReturnsCorrectDtos()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        var thirdUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(secondUser);
        _unitOfWork.Users.Add(thirdUser);

        var friendship1 = Friendship.Create(secondUser, user);
        var friendship2 = Friendship.Create(secondUser, user);

        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetPendingFriendshipsToUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, x => Assert.False(x.IsConfirmed));
    }
    
    [Fact]
    public async Task Handle_PendingAndConfirmedFriendships_ReturnsCorrectDtos()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        var thirdUser = new UserBuilder().Build();
        var fourthUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(secondUser);
        _unitOfWork.Users.Add(thirdUser);
        _unitOfWork.Users.Add(fourthUser);

        var friendship1 = Friendship.Create(user, secondUser);
        var friendship2 = Friendship.Create(user, thirdUser);
        var friendship3 = Friendship.Create(fourthUser, user);

        friendship2.ConfirmBy(thirdUser);
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);
        _unitOfWork.Friendships.Add(friendship3);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetPendingFriendshipsToUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Contains(result.Value, x => x.FirstUserId == fourthUser.Id && x.SecondUserId == user.Id && !x.IsConfirmed);
    }
}
