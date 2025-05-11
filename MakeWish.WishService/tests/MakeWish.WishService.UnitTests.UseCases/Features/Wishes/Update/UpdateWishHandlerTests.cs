using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.Update;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.Update;

public class UpdateWishHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UpdateWishHandler _handler;

    public UpdateWishHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new UpdateWishHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateWish_WhenUserIsOwnerAndWishIsCreated_WishWasAndBecomePublic()
    {
        // Arrange
        const string newTitle = "New Title";
        const string newDescription = "New Description";
        
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(owner).Build();
        var publicWishList = new WishListBuilder().WithOwner(owner).WithWishes([wish]).Build();
        var privateWishList = new WishListBuilder().WithOwner(owner).Build();
        
        owner.PublicWishListId = publicWishList.Id;
        owner.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new UpdateWishCommand(wish.Id, newTitle, newDescription, IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be(newTitle);
        wish.Description.Should().Be(newDescription);
        
        publicWishList.Wishes.Count.Should().Be(1);
        privateWishList.Wishes.Count.Should().Be(0);
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateWish_WhenUserIsOwnerAndWishIsCreated_WishWasAndBecomePrivate()
    {
        // Arrange
        const string newTitle = "New Title";
        const string newDescription = "New Description";
        
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(owner).Build();
        var publicWishList = new WishListBuilder().WithOwner(owner).Build();
        var privateWishList = new WishListBuilder().WithOwner(owner).WithWishes([wish]).Build();
        
        owner.PublicWishListId = publicWishList.Id;
        owner.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new UpdateWishCommand(wish.Id, newTitle, newDescription, IsPublic: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be(newTitle);
        wish.Description.Should().Be(newDescription);
        
        publicWishList.Wishes.Count.Should().Be(0);
        privateWishList.Wishes.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateWish_WhenUserIsOwnerAndWishIsCreated_WishWasPublicAndBecomePrivate()
    {
        // Arrange
        const string newTitle = "New Title";
        const string newDescription = "New Description";
        
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(owner).Build();
        var publicWishList = new WishListBuilder().WithOwner(owner).WithWishes([wish]).Build();
        var privateWishList = new WishListBuilder().WithOwner(owner).Build();
        
        owner.PublicWishListId = publicWishList.Id;
        owner.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new UpdateWishCommand(wish.Id, newTitle, newDescription, IsPublic: false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be(newTitle);
        wish.Description.Should().Be(newDescription);
        
        publicWishList.Wishes.Count.Should().Be(0);
        privateWishList.Wishes.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task Handle_ShouldUpdateWish_WhenUserIsOwnerAndWishIsCreated_WishWasPrivateAndBecomePublic()
    {
        // Arrange
        const string newTitle = "New Title";
        const string newDescription = "New Description";
        
        var owner = new UserBuilder().Build();
        var wish = new WishBuilder().WithOwner(owner).Build();
        var publicWishList = new WishListBuilder().WithOwner(owner).Build();
        var privateWishList = new WishListBuilder().WithOwner(owner).WithWishes([wish]).Build();
        
        owner.PublicWishListId = publicWishList.Id;
        owner.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(wish);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new UpdateWishCommand(wish.Id, newTitle, newDescription, IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be(newTitle);
        wish.Description.Should().Be(newDescription);
        
        publicWishList.Wishes.Count.Should().Be(1);
        privateWishList.Wishes.Count.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(false);
        
        var command = new UpdateWishCommand(Guid.NewGuid(), "Title", "Description", IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is AuthenticationError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishNotFound()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());
        
        var command = new UpdateWishCommand(Guid.NewGuid(), "Title", "Description", IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(Guid.NewGuid());

        var wish = new WishBuilder().Build();
        _unitOfWork.Wishes.Add(wish);

        var command = new UpdateWishCommand(wish.Id, "Title", "Description", IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenWishStatusIsNotCreated()
    {
        // Arrange
        var owner = new UserBuilder().Build();
        _unitOfWork.Users.Add(owner);
        
        var wish = new WishBuilder().WithOwner(owner).Build().PromisedBy(new UserBuilder().Build());
        _unitOfWork.Wishes.Add(wish);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(owner.Id);

        var command = new UpdateWishCommand(wish.Id, "Title", "Description", IsPublic: true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
}