using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.WishLists.GetUserWishLists;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.WishLists.GetUserWishLists;

public class GetUserWishListsHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetUserWishListsHandler _handler;

    public GetUserWishListsHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetUserWishListsHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnMyWishLists_ExceptPublicAndPrivate()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var publicWishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        owner.PublicWishListId = publicWishList.Id;
        var ownerWishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes(new[] { wish })
            .Build();
        var otherUserWishList = new WishListBuilder()
            .WithOwner(otherUser)
            .WithWishes(new[] { wish })
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(otherUser);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(ownerWishList);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(otherUserWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetUserWishListsCommand(owner.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().Contain(wl => wl.Id == ownerWishList.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnUserWishLists_WithAccess_ExceptPublicAndPrivate()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var wish1 = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wish2 = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wish3 = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var publicWishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();
        owner.PublicWishListId = publicWishList.Id;
        var ownerWishListWithAccess = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes(new[] { wish1 })
            .Build();
        var ownerWishListWithoutAccess = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes(new[] { wish2 })
            .Build();
        var otherUserWishList = new WishListBuilder()
            .WithOwner(user)
            .WithWishes(new[] { wish3 })
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        _unitOfWork.Wishes.Add(wish3);
        _unitOfWork.WishLists.Add(ownerWishListWithAccess);
        _unitOfWork.WishLists.AllowUserAccess(ownerWishListWithAccess, user);
        _unitOfWork.WishLists.Add(ownerWishListWithoutAccess);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.AllowUserAccess(publicWishList, user);
        _unitOfWork.WishLists.Add(otherUserWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetUserWishListsCommand(owner.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().Contain(wl => wl.Id == ownerWishListWithAccess.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetUserWishListsCommand(Guid.NewGuid());
        
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
        
        var command = new GetUserWishListsCommand(userId);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 