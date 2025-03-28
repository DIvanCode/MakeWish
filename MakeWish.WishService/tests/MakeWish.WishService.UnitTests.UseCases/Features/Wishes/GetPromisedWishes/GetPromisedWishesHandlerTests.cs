using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.GetPromisedWishes;

public class GetPromisedWishesHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetPromisedWishesHandler _handler;

    public GetPromisedWishesHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetPromisedWishesHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnPromisedWishes_WhenUserIsAuthenticated()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var wish1 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        var wish2 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPromisedWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(wish => wish.Status.Should().Be(WishStatus.Promised));
        result.Value.Should().AllSatisfy(wish => wish.PromiserId.Should().Be(user.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyPromisedWishes_WhenMixedWishesExist()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var promisedWish1 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        var promisedWish2 = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(user);
        var createdWish = new WishBuilder()
            .WithOwner(owner)
            .Build();
        var completedWish = new WishBuilder()
            .WithOwner(owner)
            .Build()
            .PromisedBy(owner)
            .CompletedBy(owner);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(promisedWish1);
        _unitOfWork.Wishes.Add(promisedWish2);
        _unitOfWork.Wishes.Add(createdWish);
        _unitOfWork.Wishes.Add(completedWish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPromisedWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().AllSatisfy(wish => wish.Status.Should().Be(WishStatus.Promised));
        result.Value.Should().AllSatisfy(wish => wish.PromiserId.Should().Be(user.Id));
        result.Value.Select(w => w.Id).Should().BeEquivalentTo(new[] { promisedWish1.Id, promisedWish2.Id });
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPromisedWishes()
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
            .PromisedBy(owner)
            .CompletedBy(owner);
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetPromisedWishesCommand();
        
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
        
        var command = new GetPromisedWishesCommand();
        
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
        
        var command = new GetPromisedWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 