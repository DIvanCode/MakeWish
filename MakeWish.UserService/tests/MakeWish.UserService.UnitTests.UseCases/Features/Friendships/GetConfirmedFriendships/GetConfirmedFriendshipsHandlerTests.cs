using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UnitTests.Common;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Friendships.GetConfirmedFriendships;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.GetConfirmedFriendships;

public class GetConfirmedFriendshipsHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetConfirmedFriendshipsHandler _sut;

    public GetConfirmedFriendshipsHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetConfirmedFriendshipsHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ReturnsAuthenticationError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetConfirmedFriendshipsCommand(Guid.NewGuid());

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
        
        var command = new GetConfirmedFriendshipsCommand(Guid.NewGuid());

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

        var command = new GetConfirmedFriendshipsCommand(user.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.IsType<EntityNotFoundError>(result.Errors[0]);
    }

    [Fact]
    public async Task Handle_SuccessfulFriendshipRetrieval_ReturnsFriendshipDtos()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        var friendship = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(secondUser)
            .Build()
            .ConfirmedBy(secondUser);
        
        _unitOfWork.Friendships.Add(friendship);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);

        var command = new GetConfirmedFriendshipsCommand(firstUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        var friendshipDto = result.Value.First();
        Assert.Equal(firstUser.Id, friendshipDto.FirstUserId);
        Assert.Equal(secondUser.Id, friendshipDto.SecondUserId);
        Assert.True(friendshipDto.IsConfirmed);
    }
    
    [Fact]
    public async Task Handle_MultipleConfirmedFriendships_ReturnsFriendshipDtos()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        var thirdUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);
        _unitOfWork.Users.Add(thirdUser);

        var friendship1 = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(secondUser)
            .Build()
            .ConfirmedBy(secondUser);
        
        var friendship2 = new FriendshipBuilder()
            .WithFirstUser(thirdUser)
            .WithSecondUser(firstUser)
            .Build()
            .ConfirmedBy(firstUser);
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);

        var command = new GetConfirmedFriendshipsCommand(firstUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        var friendshipDtos = result.Value.ToList();

        Assert.Contains(friendshipDtos, dto => dto.FirstUserId == firstUser.Id && dto.SecondUserId == secondUser.Id);
        Assert.Contains(friendshipDtos, dto => dto.FirstUserId == thirdUser.Id && dto.SecondUserId == firstUser.Id);
        Assert.All(friendshipDtos, dto => Assert.True(dto.IsConfirmed));
    }
    
    [Fact]
    public async Task Handle_MixedConfirmedAndNotConfirmedFriendships_ReturnsOnlyConfirmedFriendships()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();
        var thirdUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);
        _unitOfWork.Users.Add(thirdUser);

        var friendship1 = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(secondUser)
            .Build();
        
        var friendship2 = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(thirdUser)
            .Build()
            .ConfirmedBy(thirdUser);

        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(firstUser.Id);

        var command = new GetConfirmedFriendshipsCommand(firstUser.Id);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        var friendshipDto = result.Value.Single();
    
        Assert.Equal(firstUser.Id, friendshipDto.FirstUserId);
        Assert.Equal(thirdUser.Id, friendshipDto.SecondUserId);
        Assert.True(friendshipDto.IsConfirmed);
    }

}
