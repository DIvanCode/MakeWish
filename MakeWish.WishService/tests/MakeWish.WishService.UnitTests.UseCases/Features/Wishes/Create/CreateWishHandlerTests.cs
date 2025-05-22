using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.Create;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.Create;

public class CreateWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateWishHandler _handler;

    public CreateWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new CreateWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateWish_WhenUserIsAuthenticated()
    {
        // Arrange
        const string title = "New Wish";
        const string description = "Wish Description";
        const bool isPublic = true;
        
        var user = new UserBuilder().Build();
        var publicWishList = new WishListBuilder().WithOwner(user).Build();
        var privateWishList = new WishListBuilder().WithOwner(user).Build();
        
        user.PublicWishListId = publicWishList.Id;
        user.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new CreateWishCommand(title, description, isPublic);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var createdWish = await _unitOfWork.Wishes.GetByIdAsync(result.Value.Id, CancellationToken.None);
        createdWish.Should().NotBeNull();
        createdWish.Title.Should().Be(title);
        createdWish.Description.Should().Be(description);
        createdWish.Owner.Id.Should().Be(user.Id);
        createdWish.IsPublic.Should().Be(isPublic);
    }
    
    [Fact]
    public async Task Handle_ShouldCreateWish_PublicWishListShouldContainWish()
    {
        // Arrange
        const string title = "New Wish";
        const string description = "Wish Description";
        const bool isPublic = true;
        
        var user = new UserBuilder().Build();
        var publicWishList = new WishListBuilder().WithOwner(user).Build();
        var privateWishList = new WishListBuilder().WithOwner(user).Build();
        
        user.PublicWishListId = publicWishList.Id;
        user.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new CreateWishCommand(title, description, isPublic);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var createdWish = await _unitOfWork.Wishes.GetByIdAsync(result.Value.Id, CancellationToken.None);
        createdWish.Should().NotBeNull();
        createdWish.Title.Should().Be(title);
        createdWish.Description.Should().Be(description);
        createdWish.Owner.Id.Should().Be(user.Id);
        createdWish.IsPublic.Should().Be(isPublic);
        
        publicWishList.Wishes.Count.Should().Be(1);
        publicWishList.Wishes.Single().Should().Be(createdWish);

        privateWishList.Wishes.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task Handle_ShouldCreateWish_PrivateWishListShouldContainWish()
    {
        // Arrange
        const string title = "New Wish";
        const string description = "Wish Description";
        const bool isPublic = false;
        
        var user = new UserBuilder().Build();
        var publicWishList = new WishListBuilder().WithOwner(user).Build();
        var privateWishList = new WishListBuilder().WithOwner(user).Build();
        
        user.PublicWishListId = publicWishList.Id;
        user.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new CreateWishCommand(title, description, isPublic);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        var createdWish = await _unitOfWork.Wishes.GetByIdAsync(result.Value.Id, CancellationToken.None);
        createdWish.Should().NotBeNull();
        createdWish.Title.Should().Be(title);
        createdWish.Description.Should().Be(description);
        createdWish.Owner.Id.Should().Be(user.Id);
        createdWish.IsPublic.Should().Be(isPublic);
        
        publicWishList.Wishes.Count.Should().Be(0);
        privateWishList.Wishes.Count.Should().Be(1);
        privateWishList.Wishes.Single().Should().Be(createdWish);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new CreateWishCommand("New Wish", "Wish Description", IsPublic: true);
        
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
        
        var command = new CreateWishCommand("New Wish", "Wish Description", IsPublic: true);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
}