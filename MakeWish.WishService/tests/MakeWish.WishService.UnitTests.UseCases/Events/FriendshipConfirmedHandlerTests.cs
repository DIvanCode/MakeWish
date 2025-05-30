using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.UnitTests.UseCases.Events;

public class FriendshipConfirmedHandlerTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly FriendshipConfirmedHandler _sut;

    public FriendshipConfirmedHandlerTests()
    {
        _unitOfWork = new UnitOfWorkStub();
        _sut = new FriendshipConfirmedHandler(_unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAllowUsersAccessToMainWishLists()
    {
        // Arrange
        var user1 = new UserBuilder().Build();
        var user2 = new UserBuilder().Build();
        var wishList1 = new WishListBuilder().WithOwner(user1).Build();
        var wishList2 = new WishListBuilder().WithOwner(user2).Build();
        user1.PublicWishListId = wishList1.Id;
        user2.PublicWishListId = wishList2.Id;
        
        _unitOfWork.Users.Add(user1);
        _unitOfWork.Users.Add(user2);
        _unitOfWork.WishLists.Add(wishList1);
        _unitOfWork.WishLists.Add(wishList2);

        var request = new FriendshipConfirmedNotification(user1.Id, user2.Id);
        
        // Act
        await _sut.Handle(request, CancellationToken.None);

        // Assert
        var hasAccess1 = await _unitOfWork.WishLists.HasUserAccessAsync(wishList1, user2, CancellationToken.None);
        hasAccess1.Should().BeTrue();
        var hasAccess2 = await _unitOfWork.WishLists.HasUserAccessAsync(wishList2, user1, CancellationToken.None);
        hasAccess2.Should().BeTrue();
    }
}