using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UnitTests.Common.Models;
using MakeWish.UserService.UseCases.Features.Users.GetAll;
using MakeWish.UserService.UseCases.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MakeWish.UserService.UnitTests.UseCases.Features.Users.GetAll;

public class GetAllUsersHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly Mock<IServiceProvider> _serviceProviderMock = new();
    private readonly Mock<IUserContext> _userContextMock = new();
    private readonly GetAllUsersHandler _sut;

    public GetAllUsersHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(IUserContext)))
            .Returns(_userContextMock.Object);
        _sut = new GetAllUsersHandler(_unitOfWork, _serviceProviderMock.Object);
    }

    [Fact]
    public async Task Handle_WhenSearchParametersNotSet_ReturnsUserDtos()
    {
        // Arrange
        var firstUser = new UserBuilder().Build();
        var secondUser = new UserBuilder().Build();

        _unitOfWork.Users.Add(firstUser);
        _unitOfWork.Users.Add(secondUser);

        var command = new GetAllUsersCommand(null, null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        
        var userDto1 = result.Value.First();
        Assert.Equivalent(userDto1, firstUser);
        var userDto2 = result.Value.Last();
        Assert.Equivalent(userDto2, secondUser);
    }
    
    [Fact]
    public async Task Handle_WhenQuerySet_OnlyFriendsNotSet_ReturnsUserDtos()
    {
        // Arrange
        var user1 = new UserBuilder().WithName("Иван").WithSurname("Добрынин").Build();
        var user2 = new UserBuilder().WithName("Иван").WithSurname("Светлаков").Build();
        var user3 = new UserBuilder().WithName("Дмитрий").WithSurname("Добрынин").Build();

        _unitOfWork.Users.Add(user1);
        _unitOfWork.Users.Add(user2);
        _unitOfWork.Users.Add(user3);

        var command1 = new GetAllUsersCommand("Иван Добрынин", null);
        var command2 = new GetAllUsersCommand("Добрынин Иван", null);
        var command3 = new GetAllUsersCommand("Дмитрий Светлаков", null);
        var command4 = new GetAllUsersCommand("Светлаков Иван", null);

        // Act
        var result1 = await _sut.Handle(command1, CancellationToken.None);
        var result2 = await _sut.Handle(command2, CancellationToken.None);
        var result3 = await _sut.Handle(command3, CancellationToken.None);
        var result4 = await _sut.Handle(command4, CancellationToken.None);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsSuccess);
        Assert.True(result4.IsSuccess);
        
        Assert.Single(result1.Value);
        Assert.Single(result2.Value);
        
        Assert.Equivalent(result1.Value.Single(), user1);
        Assert.Equivalent(result2.Value.Single(), user1);
    }
    
    [Fact]
    public async Task Handle_WhenQueryNotSet_OnlyFriendsSet_ReturnsUserDtos()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var user1 = new UserBuilder().WithName("Иван").WithSurname("Добрынин").Build();
        var user2 = new UserBuilder().WithName("Иван").WithSurname("Светлаков").Build();
        var user3 = new UserBuilder().WithName("Дмитрий").WithSurname("Добрынин").Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(user1);
        _unitOfWork.Users.Add(user2);
        _unitOfWork.Users.Add(user3);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var friendship1 = new FriendshipBuilder().WithFirstUser(user).WithSecondUser(user1).Build();
        var friendship2 = new FriendshipBuilder().WithFirstUser(user).WithSecondUser(user3).Build();
        friendship1.ConfirmBy(user1);
        friendship2.ConfirmBy(user3);
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship2);

        var command = new GetAllUsersCommand(null, true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        
        Assert.Equal(2, result.Value.Count);
        
        var userDto1 = result.Value.First();
        Assert.Equivalent(userDto1, user1);
        var userDto2 = result.Value.Last();
        Assert.Equivalent(userDto2, user3);
    }
    
    [Fact]
    public async Task Handle_WhenOnlyFriendsSet_UserNotAuthenticated_ReturnsError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetAllUsersCommand(null, true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_WhenOnlyFriendsSet_UserNotFound_ReturnsError()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new GetAllUsersCommand(null, true);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public async Task Handle_WhenQuerySet_OnlyFriendsSet_ReturnsUserDtos()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var user1 = new UserBuilder().WithName("Иван").WithSurname("Добрынин").Build();
        var user2 = new UserBuilder().WithName("Иван").WithSurname("Светлаков").Build();
        var user3 = new UserBuilder().WithName("Дмитрий").WithSurname("Добрынин").Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(user1);
        _unitOfWork.Users.Add(user2);
        _unitOfWork.Users.Add(user3);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var friendship1 = new FriendshipBuilder().WithFirstUser(user).WithSecondUser(user1).Build();
        var friendship3 = new FriendshipBuilder().WithFirstUser(user).WithSecondUser(user3).Build();
        friendship1.ConfirmBy(user1);
        friendship1.ConfirmBy(user3);
        
        _unitOfWork.Friendships.Add(friendship1);
        _unitOfWork.Friendships.Add(friendship3);

        var command1 = new GetAllUsersCommand("Иван Добрынин", true);
        var command2 = new GetAllUsersCommand("Добрынин Иван", true);
        var command3 = new GetAllUsersCommand("Иван Светлаков", true);
        var command4 = new GetAllUsersCommand("Светлаков Иван", true);

        // Act
        var result1 = await _sut.Handle(command1, CancellationToken.None);
        var result2 = await _sut.Handle(command2, CancellationToken.None);
        var result3 = await _sut.Handle(command3, CancellationToken.None);
        var result4 = await _sut.Handle(command4, CancellationToken.None);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsSuccess);
        Assert.True(result4.IsSuccess);
        
        Assert.Single(result1.Value);
        Assert.Single(result2.Value);
        
        Assert.Equivalent(result1.Value.Single(), user1);
        Assert.Equivalent(result2.Value.Single(), user1);
    }
}
