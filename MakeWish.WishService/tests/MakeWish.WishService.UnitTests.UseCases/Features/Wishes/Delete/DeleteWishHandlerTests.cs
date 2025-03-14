using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.Delete;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.Delete;

public class DeleteWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DeleteWishHandler _handler;

    public DeleteWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new DeleteWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteWish_WhenUserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(owner.Id);
        
        var command = new DeleteWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Deleted);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(false);
        
        var command = new DeleteWishCommand(Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishNotFound()
    {
        // Arrange
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        
        var command = new DeleteWishCommand(Guid.NewGuid());
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        
        var wish = new WishBuilder().WithOwner(owner).Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        
        var command = new DeleteWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder().WithOwner(owner).Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(otherUser.Id);
        
        var command = new DeleteWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishCannotBeDeleted()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build().PromisedBy(new UserBuilder().Build());
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(owner.Id);
        
        var command = new DeleteWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishAlreadyDeleted()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build().DeletedBy(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(x => x.IsAuthenticated).Returns(true);
        _userContextMock.Setup(x => x.UserId).Returns(owner.Id);
        
        var command = new DeleteWishCommand(wish.Id);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}