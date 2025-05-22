using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.WishLists.GetUsersWithAccess;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.WishLists.GetUsersWithAccess;

public class GetUsersWithAccessToWishListHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetUsersWithAccessToWishListHandler _handler;

    public GetUsersWithAccessToWishListHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetUsersWithAccessToWishListHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersWithAccess_WhenUserIsAuthenticated()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var userWithAccess1 = new UserBuilder().Build();
        var userWithAccess2 = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();

        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(userWithAccess1);
        _unitOfWork.Users.Add(userWithAccess2);
        _unitOfWork.WishLists.Add(wishList);

        _unitOfWork.WishLists.AllowUserAccess(wishList, userWithAccess1);
        _unitOfWork.WishLists.AllowUserAccess(wishList, userWithAccess2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new GetUsersWithAccessToWishListCommand(wishList.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().Contain(u => u.Id == userWithAccess1.Id);
        result.Value.Should().Contain(u => u.Id == userWithAccess2.Id);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotOwner()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        var user = new UserBuilder().Build();
        var userWithAccess1 = new UserBuilder().Build();
        var userWithAccess2 = new UserBuilder().Build();
        var wishList = new WishListBuilder()
            .WithOwner(owner)
            .Build();

        _unitOfWork.Users.Add(owner);
        _unitOfWork.Users.Add(userWithAccess1);
        _unitOfWork.Users.Add(userWithAccess2);
        _unitOfWork.WishLists.Add(wishList);

        _unitOfWork.WishLists.AllowUserAccess(wishList, userWithAccess1);
        _unitOfWork.WishLists.AllowUserAccess(wishList, userWithAccess2);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetUsersWithAccessToWishListCommand(wishList.Id);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);

        var command = new GetUsersWithAccessToWishListCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishListNotFound()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);

        var command = new GetUsersWithAccessToWishListCommand(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
}
