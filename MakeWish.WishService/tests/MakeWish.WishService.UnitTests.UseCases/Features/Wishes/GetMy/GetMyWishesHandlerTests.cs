using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.GetMy;
using MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.GetMy;

public class GetMyWishesHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetMyWishesHandler _handler;

    public GetMyWishesHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetMyWishesHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishes_WhenUserIsAuthenticated()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var wish1 = new WishBuilder().WithOwner(owner).Build();
        var wish2 = new WishBuilder().WithOwner(owner).Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetMyWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyPromisedWishes_WhenMixedWishesExist()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var wish1 = new WishBuilder().WithOwner(owner).Build();
        var wish2 = new WishBuilder().WithOwner(owner).Build();
        var wish3 = new WishBuilder().WithOwner(user).Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        _unitOfWork.Wishes.Add(wish3);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetMyWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Select(w => w.Id).Should().BeEquivalentTo([wish1.Id, wish2.Id]);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoWishes()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(user).Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new GetMyWishesCommand();
        
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
        
        var command = new GetMyWishesCommand();
        
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
        
        var command = new GetMyWishesCommand();
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
} 