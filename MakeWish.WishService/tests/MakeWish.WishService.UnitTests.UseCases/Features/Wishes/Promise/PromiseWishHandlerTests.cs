using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.Promise;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.Promise;

public class PromiseWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PromiseWishHandler _handler;

    public PromiseWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new PromiseWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldPromiseWish_WhenUserIsAuthenticatedAndWishExists()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        
        var wish = new WishBuilder().WithOwner(owner).Build();
        
        _unitOfWork.Wishes.Add(wish);
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(promiser.Id);

        var command = new PromiseWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetPromiserFor(promiser).Should().Be(promiser);
        wish.GetStatusFor(promiser).Should().Be(WishStatus.Promised);
        wish.GetPromiserFor(owner).Should().Be(null);
        wish.GetStatusFor(owner).Should().Be(WishStatus.Created);
    }
    
    [Fact]
    public async Task Handle_ShouldPromiseWish_WhenUserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build();
        
        _unitOfWork.Wishes.Add(wish);
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new PromiseWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetPromiserFor(owner).Should().Be(owner);
        wish.GetStatusFor(owner).Should().Be(WishStatus.Promised);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new PromiseWishCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishDoesNotExist()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new PromiseWishCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserDoesNotExist()
    {
        // Arrange
        var user = new UserBuilder().Build();
        
        var wish = new WishBuilder().WithOwner(user).Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());

        var command = new PromiseWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishStatusDoesNotAllowPromise()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);

        var wish = new WishBuilder().WithOwner(user).Build().DeletedBy(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new PromiseWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}