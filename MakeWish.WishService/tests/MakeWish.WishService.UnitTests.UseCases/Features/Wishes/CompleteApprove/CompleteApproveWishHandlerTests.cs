using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.CompleteApprove;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.CompleteApprove;
    
public class CompleteApproveWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CompleteApproveWishHandler _handler;

    public CompleteApproveWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new CompleteApproveWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCompleteApproveWish_WhenUserIsAuthenticatedAndWishIsCompleted()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(otherUser)
            .CompletedBy(otherUser);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new CompleteApproveWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(WishStatus.Approved.ToString());
        result.Value.CompleterId.Should().Be(otherUser.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);

        var command = new CompleteApproveWishCommand(Guid.NewGuid());

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
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(otherUser);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(otherUser.Id);

        var command = new CompleteApproveWishCommand(Guid.NewGuid());

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
        var owner = new UserBuilder().Build();
        var otherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(otherUser);
        
        var wish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(otherUser)
            .CompletedBy(otherUser);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());

        var command = new CompleteApproveWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishStatusDoesNotAllowCompletionApproval()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(user).Build().DeletedBy(user);

        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new CompleteApproveWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}
