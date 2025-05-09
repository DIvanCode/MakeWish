using FluentAssertions;
using FluentResults;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.Utils.Errors;

namespace MakeWish.WishService.UnitTests.Models;

public class WishTests
{
    private const string Title = "title";
    private const string Description = "description";
    private const string ImageUrl = "imageUrl";
    private const string OtherTitle = "otherTitle";
    private const string OtherDescription = "otherTitle";
    private const string OtherImageUrl = "otherImageUrl";
    
    private static readonly User Owner = new UserBuilder().Build();
    private static readonly User OtherUser = new UserBuilder().Build();

    [Fact]
    public void Create_ShouldCreateWish_WithValidData()
    {
        // Arrange

        // Act
        var wish = Wish.Create(Title, Description, Owner);
        
        // Assert
        wish.Should().NotBeNull();
        wish.Title.Should().Be(Title);
        wish.Description.Should().Be(Description);
        wish.ImageUrl.Should().Be(null);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
        wish.Owner.Should().Be(Owner);
        wish.GetPromiserFor(Owner).Should().Be(null);
        wish.GetPromiserFor(OtherUser).Should().Be(null);
        wish.GetCompleter().Should().Be(null);
    }
    
    [Fact]
    public void Update_ShouldUpdateTitleAndDescription_WhenOwnerUpdates()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle(Title)
            .WithDescription(Description)
            .WithImageUrl(ImageUrl)
            .WithOwner(Owner)
            .Build();
        
        // Act
        var result = wish.Update(OtherTitle, OtherDescription, OtherImageUrl, by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be(OtherTitle);
        wish.Description.Should().Be(OtherDescription);
        wish.ImageUrl.Should().Be(OtherImageUrl);
        wish.Owner.Should().Be(Owner);
        wish.GetPromiserFor(Owner).Should().Be(null);
        wish.GetPromiserFor(OtherUser).Should().Be(null);
        wish.GetCompleter().Should().Be(null);
    }
    
    [Fact]
    public void Update_ShouldReturnError_WhenNonOwnerUpdates()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Update(OtherTitle, OtherDescription, OtherImageUrl, by: OtherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void Promise_ShouldChangeStatusToPromised()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Promise(by: OtherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(Owner).Should().BeNull();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Promised);
        wish.GetPromiserFor(OtherUser).Should().Be(OtherUser);
    }
    
    [Fact]
    public void Promise_ShouldChangeStatusToPromised_WhenOwnerPromised()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Promise(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Promised);
        wish.GetPromiserFor(Owner).Should().Be(Owner);
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Promised);
        wish.GetPromiserFor(OtherUser).Should().Be(Owner);
    }
    
    [Fact]
    public void PromiseCancel_ShouldRevertToCreated_WhenPromiserCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(OtherUser);
        
        // Act
        var result = wish.PromiseCancel(by: OtherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(Owner).Should().BeNull();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(OtherUser).Should().BeNull();
    }
    
    [Fact]
    public void PromiseCancel_ShouldRevertToCreated_WhenPromiserOwnerCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(Owner);
        
        // Act
        var result = wish.PromiseCancel(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(Owner).Should().BeNull();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(OtherUser).Should().BeNull();
    }
    
    [Fact]
    public void PromiseCancel_ShouldReturnError_WhenNonPromiserCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(Owner);
        
        // Act
        var result = wish.PromiseCancel(by: OtherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void Complete_ShouldSetStatusToCompleted_WhenPromiserCompletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(OtherUser);
        
        // Act
        var result = wish.Complete(by: OtherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Completed);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Completed);
        wish.GetCompleter().Should().Be(OtherUser);
    }
    
    [Fact]
    public void Complete_ShouldSetStatusToApproved_WhenOwnerOnlyCompleted()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Complete(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Approved);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Approved);
        wish.GetCompleter().Should().Be(Owner);
    }
    
    [Fact]
    public void Complete_ShouldSetStatusToApproved_WhenOwnerPromisedAndCompleted()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(Owner);
        
        // Act
        var result = wish.Complete(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Approved);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Approved);
        wish.GetCompleter().Should().Be(Owner);
    }
    
    [Fact]
    public void Complete_ShouldReturnError_WhenNonPromiserCompletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().PromisedBy(Owner);
        
        // Act
        var result = wish.Complete(by: OtherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void CompleteApprove_ShouldApproveCompletion_WhenOwnerApproves()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(Owner)
            .Build()
            .PromisedBy(OtherUser)
            .CompletedBy(OtherUser);
        
        // Act
        var result = wish.CompleteApprove(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Approved);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Approved);
        wish.GetCompleter().Should().Be(OtherUser);
    }
    
    [Fact]
    public void CompleteReject_ShouldRejectCompletion_WhenOwnerRejects()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(Owner)
            .Build()
            .PromisedBy(OtherUser)
            .CompletedBy(OtherUser);
        
        // Act
        var result = wish.CompleteReject(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Created);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
        wish.GetPromiserFor(OtherUser).Should().BeNull();
        wish.GetPromiserFor(Owner).Should().BeNull();
        wish.GetCompleter().Should().BeNull();
    }
    
    [Fact]
    public void Delete_ShouldSetStatusToDeleted_WhenOwnerDeletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Delete(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Deleted);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Deleted);
    }
    
    [Fact]
    public void Delete_ShouldReturnError_WhenNonOwnerDeletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.Delete(by: OtherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void Restore_ShouldRestoreToCreated_WhenOwnerRestores()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build().DeletedBy(Owner);
        
        // Act
        var result = wish.Restore(by: Owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.GetStatusFor(OtherUser).Should().Be(WishStatus.Created);
        wish.GetStatusFor(Owner).Should().Be(WishStatus.Created);
    }
    
    [Fact]
    public void Restore_ShouldReturnError_WhenNonOwnerRestores()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        wish.Delete(Owner);
        
        // Act
        var result = wish.Restore(by: OtherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void IsAccessible_ShouldReturnTrue_WhenUserIsOwner()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.IsAccessible(to: Owner, existsWishListContainingWishWithUserAccess: false);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsAccessible_ShouldReturnTrue_WhenExistsWishListContainingWishWithUserAccess()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.IsAccessible(to: OtherUser, existsWishListContainingWishWithUserAccess: true);
        
        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void IsAccessible_ShouldReturnFalse_WhenWishIsNotAccessible()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var result = wish.IsAccessible(to: OtherUser, existsWishListContainingWishWithUserAccess: false);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void StatusCreated_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(Owner).Build();
        
        // Act
        var results = new List<Result>
        {
            wish.PromiseCancel(by: Owner),
            wish.PromiseCancel(by: OtherUser),
            wish.Complete(by: OtherUser),
            wish.CompleteApprove(by: Owner),
            wish.CompleteApprove(by: OtherUser),
            wish.CompleteReject(by: OtherUser),
            wish.CompleteReject(by: Owner),
            wish.Delete(by: OtherUser),
            wish.Restore(by: Owner),
            wish.Restore(by: OtherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusPromised_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle(Title)
            .WithDescription(Description)
            .WithImageUrl(ImageUrl)
            .WithOwner(Owner)
            .Build()
            .PromisedBy(OtherUser);
        
        // Act
        var results = new List<Result>
        {
            wish.Update(OtherTitle, OtherDescription, OtherImageUrl,by: Owner),
            wish.Update(OtherTitle, OtherDescription, OtherImageUrl, by: OtherUser),
            wish.Promise(by: Owner),
            wish.Promise(by: OtherUser),
            wish.PromiseCancel(by: Owner),
            wish.Complete(by: Owner),
            wish.CompleteApprove(by: Owner),
            wish.CompleteApprove(by: OtherUser),
            wish.CompleteReject(by: OtherUser),
            wish.CompleteReject(by: Owner),
            wish.Delete(by: Owner),
            wish.Delete(by: OtherUser),
            wish.Restore(by: Owner),
            wish.Restore(by: OtherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusCompleted_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle(Title)
            .WithDescription(Description)
            .WithImageUrl(ImageUrl)
            .WithOwner(Owner)
            .Build()
            .PromisedBy(OtherUser)
            .CompletedBy(OtherUser);
        
        // Act
        var results = new List<Result>
        {
            wish.Update(OtherTitle, OtherDescription, OtherImageUrl, by: Owner),
            wish.Update(OtherTitle, OtherDescription, OtherImageUrl, by: OtherUser),
            wish.Promise(by: Owner),
            wish.Promise(by: OtherUser),
            wish.PromiseCancel(by: Owner),
            wish.PromiseCancel(by: OtherUser),
            wish.Complete(by: Owner),
            wish.Complete(by: OtherUser),
            wish.CompleteApprove(by: OtherUser),
            wish.CompleteReject(by: OtherUser),
            wish.Delete(by: Owner),
            wish.Delete(by: OtherUser),
            wish.Restore(by: Owner),
            wish.Restore(by: OtherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusApproved_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle(Title)
            .WithDescription(Description)
            .WithImageUrl(ImageUrl)
            .WithOwner(Owner)
            .Build()
            .PromisedBy(OtherUser)
            .CompletedBy(OtherUser)
            .ApprovedBy(Owner);
        
        // Act
        var results = new List<Result>
        {
            wish.Update(OtherTitle, OtherDescription, ImageUrl, by: Owner),
            wish.Update(OtherTitle, OtherDescription, ImageUrl, by: OtherUser),
            wish.Promise(by: Owner),
            wish.Promise(by: OtherUser),
            wish.PromiseCancel(by: Owner),
            wish.PromiseCancel(by: OtherUser),
            wish.Complete(by: Owner),
            wish.Complete(by: OtherUser),
            wish.CompleteApprove(by: Owner),
            wish.CompleteApprove(by: OtherUser),
            wish.CompleteReject(by: OtherUser),
            wish.CompleteReject(by: Owner),
            wish.Delete(by: Owner),
            wish.Delete(by: OtherUser),
            wish.Restore(by: Owner),
            wish.Restore(by: OtherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusDeleted_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle(Title)
            .WithDescription(Description)
            .WithImageUrl(ImageUrl)
            .WithOwner(Owner)
            .Build()
            .DeletedBy(Owner);
        
        // Act
        var results = new List<Result>
        {
            wish.Update(OtherTitle, OtherDescription, ImageUrl, by: Owner),
            wish.Update(OtherTitle, OtherDescription, ImageUrl, by: OtherUser),
            wish.Promise(by: Owner),
            wish.Promise(by: OtherUser),
            wish.PromiseCancel(by: Owner),
            wish.PromiseCancel(by: OtherUser),
            wish.Complete(by: Owner),
            wish.Complete(by: OtherUser),
            wish.CompleteApprove(by: Owner),
            wish.CompleteApprove(by: OtherUser),
            wish.CompleteReject(by: OtherUser),
            wish.CompleteReject(by: Owner),
            wish.Delete(by: Owner),
            wish.Delete(by: OtherUser),
            wish.Restore(by: OtherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
}