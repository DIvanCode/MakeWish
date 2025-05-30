using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.UnitTests.UseCases.Events;

public class UserDeletedHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserDeletedHandler _deletedHandler;

    public UserDeletedHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _deletedHandler = new UserDeletedHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var wish1 = new WishBuilder().WithOwner(user).Build();
        var wish2 = new WishBuilder().WithOwner(user).Build();
        var wishList = new WishListBuilder().WithOwner(user).WithWishes([wish2]).Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        _unitOfWork.WishLists.Add(wishList);
        
        var command = new UserDeletedNotification(user.Id);
        
        // Act
        await _deletedHandler.Handle(command, CancellationToken.None);

        // Assert
        var deletedUser = await _unitOfWork.Users.GetByIdAsync(user.Id, CancellationToken.None);
        deletedUser.Should().BeNull();
        var deletedWish1 = await _unitOfWork.Wishes.GetByIdAsync(wish1.Id, CancellationToken.None);
        deletedWish1.Should().BeNull();
        var deletedWish2 = await _unitOfWork.Wishes.GetByIdAsync(wish2.Id, CancellationToken.None);
        deletedWish2.Should().BeNull();
        var deletedWishList = await _unitOfWork.WishLists.GetByIdAsync(wishList.Id, CancellationToken.None);
        deletedWishList.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ShouldPass_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UserDeletedNotification(userId);

        // Act & Assert
        await _deletedHandler.Handle(command, CancellationToken.None);
    }
}