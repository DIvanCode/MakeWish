using FluentAssertions;
using FluentResults;
using MakeWish.WishService.Models;
using MakeWish.WishService.UnitTests.Common;
using MakeWish.WishService.UnitTests.Common.Models;
using MakeWish.WishService.Utils.Errors;
using Xunit.Abstractions;

namespace MakeWish.WishService.UnitTests.Models;

public class WishTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly User _owner = new UserBuilder().WithName("Owner").Build();
    private readonly User _otherUser = new UserBuilder().WithName("OtherUser").Build();

    public WishTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Create_ShouldCreateWish_WithValidData()
    {
        // Arrange
        const string title = "Title";
        const string description = "Description";

        // Act
        var wish = Wish.Create(title, description, _owner);
        
        // Assert
        wish.Should().NotBeNull();
        wish.Title.Should().Be(title);
        wish.Description.Should().Be(description);
        wish.Status.Should().Be(WishStatus.Created);
        wish.Owner.Should().Be(_owner);
    }
    
    [Fact]
    public void Update_ShouldUpdateTitleAndDescription_WhenOwnerUpdates()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithTitle("Old Title")
            .WithDescription("Old Description")
            .WithOwner(_owner)
            .Build();
        
        // Act
        var result = wish.Update("New Title", "New Description", _owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Title.Should().Be("New Title");
        wish.Description.Should().Be("New Description");
    }
    
    [Fact]
    public void Update_ShouldReturnError_WhenNonOwnerUpdates()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.Update("New Title", "New Description", _otherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void PromiseBy_ShouldChangeStatusToPromised()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.PromiseBy(_otherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Promised);
        wish.Promiser.Should().Be(_otherUser);
    }
    
    [Fact]
    public void PromiseBy_ShouldChangeStatusToPromised_WhenOwnerPromised()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.PromiseBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Promised);
        wish.Promiser.Should().Be(_owner);
    }
    
    [Fact]
    public void PromiseCancelBy_ShouldRevertToCreated_WhenPromiserCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_otherUser);
        
        // Act
        var result = wish.PromiseCancelBy(_otherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Created);
        wish.Promiser.Should().BeNull();
    }
    
    [Fact]
    public void PromiseCancelBy_ShouldRevertToCreated_WhenPromiserOwnerCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_owner);
        
        // Act
        var result = wish.PromiseCancelBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Created);
        wish.Promiser.Should().BeNull();
    }
    
    [Fact]
    public void PromiseCancelBy_ShouldReturnError_WhenNonPromiserCancels()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_owner);
        
        // Act
        var result = wish.PromiseCancelBy(_otherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void CompleteBy_ShouldSetStatusToCompleted_WhenPromiserCompletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_otherUser);
        
        // Act
        var result = wish.CompleteBy(_otherUser);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Completed);
        wish.Completer.Should().Be(_otherUser);
    }
    
    [Fact]
    public void CompleteBy_ShouldSetStatusToApproved_WhenOwnerOnlyCompleted()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.CompleteBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Approved);
        wish.Completer.Should().Be(_owner);
    }
    
    [Fact]
    public void CompleteBy_ShouldSetStatusToApproved_WhenOwnerPromisedAndCompleted()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_owner);
        
        // Act
        var result = wish.CompleteBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Approved);
        wish.Completer.Should().Be(_owner);
    }
    
    [Fact]
    public void CompleteApproveBy_ShouldApproveCompletion_WhenOwnerApproves()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(_owner)
            .Build()
            .PromisedBy(_otherUser)
            .CompletedBy(_otherUser);
        
        // Act
        var result = wish.CompleteApproveBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Approved);
    }
    
    [Fact]
    public void DeleteBy_ShouldSetStatusToDeleted_WhenOwnerDeletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.DeleteBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Deleted);
    }
    
    [Fact]
    public void DeleteBy_ShouldReturnError_WhenNonOwnerDeletes()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var result = wish.DeleteBy(_otherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }
    
    [Fact]
    public void RestoreBy_ShouldRestoreToCreated_WhenOwnerRestores()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().DeletedBy(_owner);
        
        // Act
        var result = wish.RestoreBy(_owner);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        wish.Status.Should().Be(WishStatus.Created);
    }
    
    [Fact]
    public void RestoreBy_ShouldReturnError_WhenNonOwnerRestores()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        wish.DeleteBy(_owner);
        
        // Act
        var result = wish.RestoreBy(_otherUser);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is ForbiddenError);
    }

    [Fact]
    public void StatusCreated_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build();
        
        // Act
        var results = new List<Result>
        {
            wish.PromiseCancelBy(_owner),
            wish.PromiseCancelBy(_otherUser),
            wish.CompleteBy(_otherUser),
            wish.CompleteApproveBy(_owner),
            wish.CompleteApproveBy(_otherUser),
            wish.DeleteBy(_otherUser),
            wish.RestoreBy(_owner),
            wish.RestoreBy(_otherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusPromised_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder().WithOwner(_owner).Build().PromisedBy(_otherUser);
        
        // Act
        var results = new List<Result>
        {
            wish.Update("title", "description", _owner),
            wish.Update("title", "description", _otherUser),
            wish.PromiseBy(_owner),
            wish.PromiseBy(_otherUser),
            wish.PromiseCancelBy(_owner),
            wish.CompleteBy(_owner),
            wish.CompleteApproveBy(_owner),
            wish.CompleteApproveBy(_otherUser),
            wish.DeleteBy(_owner),
            wish.DeleteBy(_otherUser),
            wish.RestoreBy(_owner),
            wish.RestoreBy(_otherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusCompleted_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(_owner)
            .Build()
            .PromisedBy(_otherUser)
            .CompletedBy(_otherUser);
        
        // Act
        var results = new List<Result>
        {
            wish.Update("title", "description", _owner),
            wish.Update("title", "description", _otherUser),
            wish.PromiseBy(_owner),
            wish.PromiseBy(_otherUser),
            wish.PromiseCancelBy(_owner),
            wish.PromiseCancelBy(_otherUser),
            wish.CompleteBy(_owner),
            wish.CompleteBy(_otherUser),
            wish.CompleteApproveBy(_otherUser),
            wish.DeleteBy(_owner),
            wish.DeleteBy(_otherUser),
            wish.RestoreBy(_owner),
            wish.RestoreBy(_otherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusApproved_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(_owner)
            .Build()
            .PromisedBy(_otherUser)
            .CompletedBy(_otherUser)
            .ApprovedBy(_owner);
        
        // Act
        var results = new List<Result>
        {
            wish.Update("title", "description", _owner),
            wish.Update("title", "description", _otherUser),
            wish.PromiseBy(_owner),
            wish.PromiseBy(_otherUser),
            wish.PromiseCancelBy(_owner),
            wish.PromiseCancelBy(_otherUser),
            wish.CompleteBy(_owner),
            wish.CompleteBy(_otherUser),
            wish.CompleteApproveBy(_owner),
            wish.CompleteApproveBy(_otherUser),
            wish.DeleteBy(_owner),
            wish.DeleteBy(_otherUser),
            wish.RestoreBy(_owner),
            wish.RestoreBy(_otherUser)
        };
        
        // Assert
        _testOutputHelper.WriteLine(results.Select(r => r.IsSuccess).ToList().IndexOf(true).ToString());
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
    
    [Fact]
    public void StatusDeleted_ShouldReturnError_WhenInvalidActions()
    {
        // Arrange
        var wish = new WishBuilder()
            .WithOwner(_owner)
            .Build()
            .DeletedBy(_owner);
        
        // Act
        var results = new List<Result>
        {
            wish.Update("title", "description", _owner),
            wish.Update("title", "description", _otherUser),
            wish.PromiseBy(_owner),
            wish.PromiseBy(_otherUser),
            wish.PromiseCancelBy(_owner),
            wish.PromiseCancelBy(_otherUser),
            wish.CompleteBy(_owner),
            wish.CompleteBy(_otherUser),
            wish.CompleteApproveBy(_owner),
            wish.CompleteApproveBy(_otherUser),
            wish.DeleteBy(_owner),
            wish.DeleteBy(_otherUser),
            wish.RestoreBy(_otherUser)
        };
        
        // Assert
        results.Where(result => result.IsSuccess).Should().BeEmpty();
    }
}