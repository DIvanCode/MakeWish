using FluentAssertions;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.Restore;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.Restore;

public class RestoreWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly UnitOfWorkStub _unitOfWork;
    private readonly RestoreWishHandler _handler;

    public RestoreWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new RestoreWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldRestoreWish_WhenUserIsOwnerAndWishIsDeleted()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build().DeletedBy(user);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new RestoreWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Created);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthenticationError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new RestoreWishCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenWishDoesNotExist()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new RestoreWishCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenUserDoesNotExist()
    {
        // Arrange
        var wish = new WishBuilder().Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new RestoreWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbiddenError_WhenUserIsNotOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder().WithOwner(owner).Build().DeletedBy(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(otherUser.Id);
        
        var command = new RestoreWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnForbiddenError_WhenWishIsNotDeleted()
    {
        // Arrange
        var user = new UserBuilder().Build();
        _unitOfWork.Users.Add(user);
        
        var wish = new WishBuilder().WithOwner(user).Build();
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new RestoreWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}