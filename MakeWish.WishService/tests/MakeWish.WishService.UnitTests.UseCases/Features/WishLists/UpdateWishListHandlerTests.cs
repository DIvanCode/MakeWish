using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Abstractions.Features.WishLists;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.UseCases.Features.WishLists;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.WishLists;

public class UpdateWishListHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateWishListHandler _handler;

    public UpdateWishListHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new UpdateWishListHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateWishList_WhenUserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes(new[] { wish })
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(wishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new UpdateWishListCommand(wishList.Id, "Updated Title");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(wishList.Id);
        result.Value.Title.Should().Be("Updated Title");
        result.Value.Owner.Id.Should().Be(owner.Id);
        result.Value.Wishes.Should().BeEmpty();
        
        var updatedWishList = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        updatedWishList.Should().NotBeNull();
        updatedWishList.Title.Should().Be("Updated Title");
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
            .WithWishes(new[] { wish })
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(wishList);
        
        // Настраиваем доступ пользователя к списку желаний
        _unitOfWork.WishLists.AllowUserAccess(wishList, user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new UpdateWishListCommand(wishList.Id, "Updated Title");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
        
        var wishListAfterUpdate = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        wishListAfterUpdate.Should().NotBeNull();
        wishListAfterUpdate.Title.Should().Be(wishList.Title);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new UpdateWishListCommand(Guid.NewGuid(), "Updated Title");
        
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
        
        var command = new UpdateWishListCommand(Guid.NewGuid(), "Updated Title");
        
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
        
        var command = new UpdateWishListCommand(Guid.NewGuid(), "Updated Title");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 