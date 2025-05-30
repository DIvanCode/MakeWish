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

public class PromiseCancelWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PromiseCancelWishHandler _handler;

    public PromiseCancelWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new PromiseCancelWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCancelPromise_WhenUserIsPromiser()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        
        var wish = new WishBuilder().WithOwner(owner).Build().PromisedBy(promiser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(promiser.Id);
        
        var command = new PromiseCancelWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedWish = await _unitOfWork.Wishes.GetByIdAsync(wish.Id, CancellationToken.None);
        updatedWish!.GetStatusFor(promiser).Should().Be(WishStatus.Created);
        updatedWish.GetPromiserFor(promiser).Should().BeNull();
        updatedWish!.GetStatusFor(owner).Should().Be(WishStatus.Created);
        updatedWish.GetPromiserFor(owner).Should().BeNull();
    }
    
    [Fact]
    public async Task Handle_ShouldCancelPromise_WhenPromiserIsOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build().PromisedBy(owner);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);
        
        var command = new PromiseCancelWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var updatedWish = await _unitOfWork.Wishes.GetByIdAsync(wish.Id, CancellationToken.None);
        updatedWish!.GetStatusFor(owner).Should().Be(WishStatus.Created);
        updatedWish.GetPromiserFor(owner).Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotPromiser()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var promiser = new UserBuilder().Build();
        var anotherUser = new UserBuilder().Build();
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(promiser);
        _unitOfWork.Users.Add(anotherUser);
        
        var wish = new WishBuilder().WithOwner(owner).Build().PromisedBy(promiser);
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(anotherUser.Id);
        
        var command = new PromiseCancelWishCommand(wish.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}