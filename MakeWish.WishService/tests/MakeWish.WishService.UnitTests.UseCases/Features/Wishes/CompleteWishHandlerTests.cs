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
    
public class CompleteWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CompleteWishHandler _handler;

    public CompleteWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new CompleteWishHandler(_userContextMock.Object, _unitOfWork);
    }
    
    [Fact]
    public async Task Handle_ShouldCompleteWish_WhenWishIsPromised()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder().WithOwner(user).Build().PromisedBy(otherUser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(otherUser.Id);

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Completed.ToString());
        result.Value.Completer.Should().NotBeNull();
        result.Value.Completer.Id.Should().Be(otherUser.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldCompleteApproveWish_WhenWishIsCompletedByOwner()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build().PromisedBy(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Approved.ToString());
        result.Value.Completer.Should().NotBeNull();
        result.Value.Completer.Id.Should().Be(user.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldCompleteApproveWish_WhenWishIsPromisedAndCompletedByOwner()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build().PromisedBy(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Approved.ToString());
        result.Value.Completer.Should().NotBeNull();
        result.Value.Completer.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);

        var command = new CompleteWishCommand(Guid.NewGuid());

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
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new CompleteWishCommand(Guid.NewGuid());

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
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build().PromisedBy(user);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishStatusDoesNotAllowCompletion()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build().DeletedBy(user);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenOtherUserPromised()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder().WithOwner(user).Build().PromisedBy(otherUser);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new CompleteWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}
