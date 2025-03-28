using FluentAssertions;
using FluentResults;
using MakeWish.WishService.Models;
using MakeWish.WishService.Utils.Errors;
using MakeWish.WishService.UnitTests.Common.Models;

namespace MakeWish.WishService.UnitTests.Models;

public class WishListTests
{
    private const string Title = "WishList title";
    private const string NewTitle = "WishList new title";

    private static readonly User Owner = new UserBuilder().Build();
    private static readonly User OtherUser = new UserBuilder().Build();
    private static readonly Wish Wish = new WishBuilder().WithOwner(Owner).Build();
    
    [Fact]
    public void Create_ShouldReturnWishList_WhenValidParameters()
    {
        // Arrange
        
        // Act
        var wishList = WishList.Create(Title, Owner);
        
        // Assert
        wishList.Should().NotBeNull();
        wishList.Title.Should().Be(Title);
        wishList.Owner.Should().Be(Owner);
        wishList.Wishes.Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldUpdateTitle_WhenOwnerIsCorrect()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.Update(NewTitle, Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wishList.Title.Should().Be(NewTitle);
    }

    [Fact]
    public void Update_ShouldReturnForbiddenError_WhenUserIsNotOwner()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.Update(NewTitle, OtherUser);
        
        // Assert
        result.Should().BeOfType<Result>().Which.Errors.Should().Contain(e => e is ForbiddenError);
    }

    [Fact]
    public void Add_ShouldAddWish_WhenOwnerIsCorrect()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.Add(Wish, Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wishList.Wishes.Should().Contain(Wish);
    }

    [Fact]
    public void Add_ShouldReturnForbiddenError_WhenUserIsNotOwner()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.Add(Wish, OtherUser);
        
        // Assert
        result.Should().BeOfType<Result>().Which.Errors.Should().Contain(e => e is ForbiddenError);
    }
    
    [Fact]
    public void Add_ShouldReturnEntityAlreadyExistsError_WhenWishAlreadyInList()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        wishList.Add(Wish, Owner);
        
        // Act
        var result = wishList.Add(Wish, Owner);
        
        // Assert
        result.Should().BeOfType<Result>().Which.Errors.Should().Contain(e => e is EntityAlreadyExistsError);
    }

    [Fact]
    public void Remove_ShouldRemoveWish_WhenOwnerIsCorrect()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        wishList.Add(Wish, Owner);
        
        // Act
        var result = wishList.Remove(Wish, Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wishList.Wishes.Should().NotContain(Wish);
    }

    [Fact]
    public void Remove_ShouldReturnForbiddenError_WhenUserIsNotOwner()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        wishList.Add(Wish, Owner);
        
        // Act
        var result = wishList.Remove(Wish, OtherUser);
        
        // Assert
        result.Should().BeOfType<Result>().Which.Errors.Should().Contain(e => e is ForbiddenError);
    }

    [Fact]
    public void Remove_ShouldReturnEntityNotFoundError_WhenWishNotInList()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.Remove(Wish, Owner);
        
        // Assert
        result.Should().BeOfType<Result>().Which.Errors.Should().Contain(e => e is EntityNotFoundError);
    }
    
    [Fact]
    public void CanUserManageAccess_ShouldReturnTrue_WhenUserIsOwner()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.CanUserManageAccess(Owner);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CanUserManageAccess_ShouldReturnFalse_WhenUserIsNotOwner()
    {
        // Arrange
        var wishList = WishList.Create(Title, Owner);
        
        // Act
        var result = wishList.CanUserManageAccess(OtherUser);
        
        // Assert
        result.Should().BeFalse();
    }
}