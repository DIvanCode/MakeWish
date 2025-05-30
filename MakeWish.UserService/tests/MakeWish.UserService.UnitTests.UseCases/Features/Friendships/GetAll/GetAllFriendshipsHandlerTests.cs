using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Friendships.GetAll;
using MakeWish.UserService.UseCases.Services;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Friendships.GetAll;

public class GetAllFriendshipsHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly GetAllFriendshipsHandler _sut;

    public GetAllFriendshipsHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _sut = new GetAllFriendshipsHandler(_unitOfWork, _userContextMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulFriendshipRetrieval_ReturnsFriendshipDtos()
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
            .WithFirstUser(secondUser)
            .WithSecondUser(thirdUser)
            .Build()
            .ConfirmedBy(thirdUser);
        var friendship3 = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(thirdUser)
            .Build();
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);
        _unitOfWork.Friendships.Add(friendship3);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.IsAdmin).Returns(true);

        var command = new GetAllFriendshipsCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        
        var friendshipDto1 = result.Value.First();
        Assert.Equal(firstUser.Id, friendshipDto1.FirstUser.Id);
        Assert.Equal(secondUser.Id, friendshipDto1.SecondUser.Id);
        Assert.True(friendshipDto1.IsConfirmed);
        
        var friendshipDto2 = result.Value.Last();
        Assert.Equal(secondUser.Id, friendshipDto2.FirstUser.Id);
        Assert.Equal(thirdUser.Id, friendshipDto2.SecondUser.Id);
        Assert.True(friendshipDto2.IsConfirmed);
    }
    
    [Fact]
    public async Task Handle_UserIsNotAdmin_Fails()
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
            .WithFirstUser(secondUser)
            .WithSecondUser(thirdUser)
            .Build()
            .ConfirmedBy(thirdUser);
        var friendship3 = new FriendshipBuilder()
            .WithFirstUser(firstUser)
            .WithSecondUser(thirdUser)
            .Build();
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);
        _unitOfWork.Friendships.Add(friendship3);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.IsAdmin).Returns(false);

        var command = new GetAllFriendshipsCommand();

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
}
