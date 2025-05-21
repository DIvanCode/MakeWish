using FluentAssertions;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UnitTests.Common.DataAccess;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.UseCases.Features.Wishes.GetUserWishes;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using Moq;

namespace MakeWish.WishService.UnitTests.UseCases.Features.Wishes.GetUserWishes;

public class GetUserWishesHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly IUnitOfWork _unitOfWork;
    private readonly GetUserWishesHandler _handler;

    public GetUserWishesHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _unitOfWork = new UnitOfWorkStub();
        _handler = new GetUserWishesHandler(_userContextMock.Object, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnWishes_WhenUserRequestsHisWishes_WhenUserIsAuthenticated()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var wish1 = new WishBuilder().WithOwner(user).Build();
        var wish2 = new WishBuilder().WithOwner(user).Build();
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetUserWishesCommand(user.Id, Query: null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnPublicWishes_WhenUserIsAuthenticated()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();
        
        var publicWishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes([new WishBuilder().WithOwner(owner).Build()])
            .Build();
        var privateWishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes([new WishBuilder().WithOwner(owner).Build()])
            .Build();
        var otherWishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes([new WishBuilder().WithOwner(owner).Build()])
            .Build();
        
        owner.PublicWishListId = publicWishList.Id;
        owner.PrivateWishListId = privateWishList.Id;
        
        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        
        publicWishList.Wishes.ToList().ForEach(wish => _unitOfWork.Wishes.Add(wish));
        privateWishList.Wishes.ToList().ForEach(wish => _unitOfWork.Wishes.Add(wish));
        otherWishList.Wishes.ToList().ForEach(wish => _unitOfWork.Wishes.Add(wish));
        
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.Add(privateWishList);
        _unitOfWork.WishLists.Add(otherWishList);
        
        _unitOfWork.WishLists.AllowUserAccess(publicWishList, user);
        _unitOfWork.WishLists.AllowUserAccess(otherWishList, user);
        
        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);
        
        var command = new GetUserWishesCommand(owner.Id, Query: null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.ToList().Single().Id.Should().Be(publicWishList.Wishes.Single().Id);
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
        
        var command = new GetUserWishesCommand(owner.Id, Query: null);
        
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
        
        var command = new GetUserWishesCommand(Guid.NewGuid(), Query: null);
        
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
        
        var command = new GetUserWishesCommand(Guid.NewGuid(), Query: null);
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is EntityNotFoundError);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnFilteredWishes_WhenQueryIsProvided_AndUserIsOwner()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var wish1 = new WishBuilder().WithOwner(user).WithTitle("Buy a new bike").Build();
        var wish2 = new WishBuilder().WithOwner(user).WithTitle("Buy chocolate").Build();
        var wish3 = new WishBuilder().WithOwner(user).WithTitle("Travel to Japan").Build();
        var wish4 = new WishBuilder().WithOwner(user).WithTitle("buy apple").Build();

        _unitOfWork.Users.Add(user);
        _unitOfWork.Wishes.Add(wish1);
        _unitOfWork.Wishes.Add(wish2);
        _unitOfWork.Wishes.Add(wish3);
        _unitOfWork.Wishes.Add(wish4);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetUserWishesCommand(user.Id, Query: "Buy");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value.Select(w => w.Id).Should().BeEquivalentTo([wish1.Id, wish2.Id, wish4.Id]);
    }

    [Fact]
    public async Task Handle_ShouldReturnFilteredPublicWishes_WhenQueryIsProvided()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var owner = new UserBuilder().Build();

        var matchingWish = new WishBuilder().WithOwner(owner).WithTitle("Buy Lego").Build();
        var nonMatchingWish = new WishBuilder().WithOwner(owner).WithTitle("Go hiking").Build();

        var publicWishList = new WishListBuilder()
            .WithOwner(owner)
            .WithWishes([matchingWish, nonMatchingWish])
            .Build();

        owner.PublicWishListId = publicWishList.Id;

        _unitOfWork.Users.Add(user);
        _unitOfWork.Users.Add(owner);
        _unitOfWork.Wishes.Add(matchingWish);
        _unitOfWork.Wishes.Add(nonMatchingWish);
        _unitOfWork.WishLists.Add(publicWishList);
        _unitOfWork.WishLists.AllowUserAccess(publicWishList, user);

        _userContextMock.Setup(uc => uc.IsAuthenticated).Returns(true);
        _userContextMock.Setup(uc => uc.UserId).Returns(user.Id);

        var command = new GetUserWishesCommand(owner.Id, Query: "Lego");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Single().Id.Should().Be(matchingWish.Id);
    }
} 