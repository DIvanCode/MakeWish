using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.WishLists.AllowAccess;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.WishLists.AllowAccess;

public class AllowUserAccessToWishListHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AllowUserAccessToWishListHandler _handler;

    public AllowUserAccessToWishListHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new AllowUserAccessToWishListHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAllowUserAccess_WhenUserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var targetUser = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(targetUser);
        _unitOfWork.WishLists.Add(wishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new AllowUserAccessToWishListCommand(wishList.Id, targetUser.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var hasAccess = await _unitOfWork.WishLists.HasUserAccessAsync(wishList, targetUser, CancellationToken.None);
        hasAccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenUserAlreadyHasAccess()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var targetUser = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(targetUser);
        _unitOfWork.WishLists.Add(wishList);
        
        // Настраиваем доступ пользователя к списку желаний
        _unitOfWork.WishLists.AllowUserAccess(wishList, targetUser);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new AllowUserAccessToWishListCommand(wishList.Id, targetUser.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var hasAccess = await _unitOfWork.WishLists.HasUserAccessAsync(wishList, targetUser, CancellationToken.None);
        hasAccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var targetUser = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(targetUser);
        _unitOfWork.WishLists.Add(wishList);
        
        // Настраиваем доступ пользователя к списку желаний
        _unitOfWork.WishLists.AllowUserAccess(wishList, user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new AllowUserAccessToWishListCommand(wishList.Id, targetUser.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
        
        var hasAccess = await _unitOfWork.WishLists.HasUserAccessAsync(wishList, targetUser, CancellationToken.None);
        hasAccess.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new AllowUserAccessToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(userId);
        
        var command = new AllowUserAccessToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishListNotFound()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new AllowUserAccessToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTargetUserNotFound()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.WishLists.Add(wishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new AllowUserAccessToWishListCommand(wishList.Id, Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 