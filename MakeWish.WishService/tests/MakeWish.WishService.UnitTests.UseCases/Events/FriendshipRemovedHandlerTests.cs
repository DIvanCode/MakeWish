using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.UnitTests.UseCases.Events;

public class FriendshipRemovedHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly FriendshipRemovedHandler _sut;

    public FriendshipRemovedHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _sut = new FriendshipRemovedHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDenyUsersAccessToMainWishLists()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var wishList1 = new WishListBuilder().WithOwner(user1).IsMain().Build();
        var wishList2 = new WishListBuilder().WithOwner(user2).IsMain().Build();
        
        _unitOfWork.Users.Add(user1);
        _unitOfWork.Users.Add(user2);
        _unitOfWork.WishLists.Add(wishList1);
        _unitOfWork.WishLists.Add(wishList2);
        _unitOfWork.WishLists.AllowUserAccess(wishList1, user2);
        _unitOfWork.WishLists.AllowUserAccess(wishList2, user1);

        var request = new FriendshipRemovedNotification(user1.Id, user2.Id);
        
        // Act
        await _sut.Handle(request, CancellationToken.None);

        // Assert
        var hasAccess1 = await _unitOfWork.WishLists.HasUserAccessAsync(wishList1, user2, CancellationToken.None);
        hasAccess1.Should().BeFalse();
        var hasAccess2 = await _unitOfWork.WishLists.HasUserAccessAsync(wishList2, user1, CancellationToken.None);
        hasAccess2.Should().BeFalse();
    }
}