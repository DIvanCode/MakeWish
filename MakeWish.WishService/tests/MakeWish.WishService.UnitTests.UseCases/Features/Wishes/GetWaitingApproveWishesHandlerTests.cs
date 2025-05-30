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

public class GetWaitingApproveWishesHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetWaitingApproveWishesHandler _handler;

    public GetWaitingApproveWishesHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetWaitingApproveWishesHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnWaitingApproveWishes_WhenUserIsAuthenticated()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var wish1 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user);
        var wish2 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWaitingApproveWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(wish => wish.Status.Should().Be(WishStatus.Completed.ToString()));
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyWaitingApproveWishes_WhenMixedWishesExist()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var waitingApproveWish1 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user);
        var waitingApproveWish2 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user);
        var createdWish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var promisedWish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        var completedByOwnerWish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(owner)
            .CompletedBy(owner);
        var approvedWish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user)
            .ApprovedBy(owner);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(waitingApproveWish1);
        _unitOfWork.Wishes.Add(waitingApproveWish2);
        _unitOfWork.Wishes.Add(createdWish);
        _unitOfWork.Wishes.Add(promisedWish);
        _unitOfWork.Wishes.Add(completedByOwnerWish);
        _unitOfWork.Wishes.Add(approvedWish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetWaitingApproveWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(wish => wish.Status.Should().Be(WishStatus.Completed.ToString()));
        result.Value.Select(w => w.Id).Should().BeEquivalentTo(new[] { waitingApproveWish1.Id, waitingApproveWish2.Id });
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoWaitingApproveWishes()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var wish1 = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var wish2 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        var wish3 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(owner)
            .CompletedBy(owner);
        var wish4 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user)
            .CompletedBy(user)
            .ApprovedBy(owner);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        _unitOfWork.Wishes.Add(wish3);
        _unitOfWork.Wishes.Add(wish4);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetWaitingApproveWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new GetWaitingApproveWishesCommand();
        
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
        
        var command = new GetWaitingApproveWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 