using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFromUser;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.GetPendingFriendshipsFromUser;

public class GetPendingFriendshipsFromUserHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetPendingFriendshipsFromUserHandler _sut;

    public GetPendingFriendshipsFromUserHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetPendingFriendshipsFromUserHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetPendingFriendshipsFromUserCommand(Guid.NewGuid());

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
        
        var command = new GetPendingFriendshipsFromUserCommand(Guid.NewGuid());

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
        
        var command = new GetPendingFriendshipsFromUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_NoPendingFriendships_ReturnsEmptyList()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(user);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPendingFriendshipsFromUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_HasPendingFriendships_ReturnsPendingFriendships()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        var thirdUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(secondUser);
        _unitOfWork.Users.Add(thirdUser);

        var friendship1 = new FriendshipBuilder()
            .WithFirstUser(user)
            .WithSecondUser(thirdUser)
            .Build();
        
        var friendship2 = new FriendshipBuilder()
            .WithFirstUser(user)
            .WithSecondUser(thirdUser)
            .Build();
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPendingFriendshipsFromUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.All(result.Value, x => Assert.False(x.IsConfirmed));
    }
    
    [Fact]
    public async Task Handle_MixedPendingAndConfirmedFriendships_ReturnsPendingFriendships()
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

        var friendship1 = new FriendshipBuilder()
            .WithFirstUser(secondUser)
            .WithSecondUser(user)
            .Build();
        
        var friendship2 = new FriendshipBuilder()
            .WithFirstUser(user)
            .WithSecondUser(thirdUser)
            .Build()
            .ConfirmedBy(thirdUser);
        
        var friendship3 = new FriendshipBuilder()
            .WithFirstUser(user)
            .WithSecondUser(fourthUser)
            .Build();
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);
        _unitOfWork.Friendships.Add(friendship3);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetPendingFriendshipsFromUserCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.All(result.Value, x => Assert.False(x.IsConfirmed));
        Assert.All(result.Value, x => Assert.Equal(x.FirstUser.Id, user.Id));
    }
}
