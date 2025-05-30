using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Abstractions.Features.Wishes;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.UseCases.Features.Wishes;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes;

public class GetWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetWishHandler _handler;

    public GetWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnWish_WhenUserIsOwner()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(user)
            .Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(wish.Id);
        result.Value.Title.Should().Be(wish.Title);
        result.Value.Description.Should().Be(wish.Description);
        result.Value.Owner.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnWish_WhenUserHasAccessThroughWishList()
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
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(wish.Id);
        result.Value.Title.Should().Be(wish.Title);
        result.Value.Description.Should().Be(wish.Description);
        result.Value.Owner.Id.Should().Be(owner.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserHasNoAccessToWishList()
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
        
        // Явно запрещаем доступ пользователя к списку желаний
        _unitOfWork.WishLists.DenyUserAccess(wishList, user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishWithCreatedStatus_WhenOwnerPromisedOwnWish()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(owner);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Promised.ToString());
        result.Value.Promiser.Should().NotBeNull();
        result.Value.Promiser.Id.Should().Be(owner.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishWithPromisedStatus_WhenOwnerPromisedByOtherUser()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(promiser);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Created.ToString());
        result.Value.Promiser.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotOwnerAndHasNoAccess()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(promiser);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishWithActualStatus_WhenWishIsCompleted()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(promiser)
            .CompletedBy(promiser);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Completed.ToString());
        result.Value.Promiser.Should().NotBeNull();
        result.Value.Promiser.Id.Should().Be(promiser.Id);
        result.Value.Completer.Should().NotBeNull();
        result.Value.Completer.Id.Should().Be(promiser.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishWithActualStatus_WhenWishIsApproved()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(promiser)
            .CompletedBy(promiser)
            .ApprovedBy(owner);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Approved.ToString());
        result.Value.Promiser.Should().NotBeNull();
        result.Value.Promiser.Id.Should().Be(promiser.Id);
        result.Value.Completer.Should().NotBeNull();
        result.Value.Completer.Id.Should().Be(promiser.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishWithActualStatus_WhenWishIsDeleted()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .DeletedBy(owner);
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Deleted.ToString());
        result.Value.Promiser.Should().BeNull();
        result.Value.Completer.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserHasNoAccess()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetWishCommand(Guid.NewGuid());
        
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
        
        var command = new GetWishCommand(Guid.NewGuid());
        
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
        var user = new UserBuilder().Build();
        var wishId = Guid.NewGuid();
        
        _unitOfWork.Users.Add(user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWishCommand(wishId);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 