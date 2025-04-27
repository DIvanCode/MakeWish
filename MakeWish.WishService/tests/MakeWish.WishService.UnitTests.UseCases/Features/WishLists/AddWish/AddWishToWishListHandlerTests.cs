using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.WishLists.AddWish;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.WishLists.AddWish;

public class AddWishToWishListHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly AddWishToWishListHandler _handler;

    public AddWishToWishListHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new AddWishToWishListHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddWishToWishList_WhenUserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(wishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new AddWishToWishListCommand(wishList.Id, wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(wishList.Id);
        result.Value.Title.Should().Be(wishList.Title);
        result.Value.OwnerId.Should().Be(owner.Id);
        result.Value.Wishes.Should().HaveCount(1);
        result.Value.Wishes[0].Id.Should().Be(wish.Id);
        result.Value.Wishes[0].Title.Should().Be(wish.Title);
        result.Value.Wishes[0].Status.Should().Be(WishStatus.Created);
        result.Value.Wishes[0].OwnerId.Should().Be(owner.Id);
        
        var updatedWishList = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        updatedWishList.Should().NotBeNull();
        updatedWishList.Wishes.Should().HaveCount(1);
        updatedWishList.Wishes[0].Id.Should().Be(wish.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(wishList);
        
        // Настраиваем доступ пользователя к списку желаний
        _unitOfWork.WishLists.AllowUserAccess(wishList, user, CancellationToken.None);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new AddWishToWishListCommand(wishList.Id, wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
        
        var wishListAfterUpdate = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        wishListAfterUpdate.Should().NotBeNull();
        wishListAfterUpdate.Wishes.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new AddWishToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
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
        
        var command = new AddWishToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
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
        
        var command = new AddWishToWishListCommand(Guid.NewGuid(), Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishNotFound()
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
        
        var command = new AddWishToWishListCommand(wishList.Id, Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
        
        var wishListAfterUpdate = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        wishListAfterUpdate.Should().NotBeNull();
        wishListAfterUpdate!.Wishes.Should().BeEmpty();
    }
} 