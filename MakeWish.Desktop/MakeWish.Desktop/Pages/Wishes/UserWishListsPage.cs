using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Wishes;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

internal sealed partial class UserWishListsPage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IUserServiceClient userServiceClient,
    IWishServiceClient wishServiceClient,
    Guid userId)
    : PageBase
{
    [ObservableProperty]
    private User _user = null!;
    
    [ObservableProperty]
    private bool _showUserDisplayName;
    
    [ObservableProperty]
    private string _wishesButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private string _wishListsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<WishList> _wishLists = [];

    [ObservableProperty]
    private bool _showPromisedWishesButton;
    
    [ObservableProperty]
    private string _promisedWishesButtonDisplayText = string.Empty;

    [ObservableProperty]
    private bool _showCreateWishListButton;
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var isCurrentUser = userId == userContext.UserId;
        
        ShowUserDisplayName = !isCurrentUser;
        ShowPromisedWishesButton = isCurrentUser;
        ShowCreateWishListButton = isCurrentUser;
        
        var userResult = await userServiceClient.GetUserAsync(userId, cancellationToken);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;
        
        var wishesResult = await wishServiceClient.GetUserWishesAsync(userId, cancellationToken);
        if (wishesResult.IsFailed)
        {
            return wishesResult.ToResult();
        }

        WishesButtonDisplayText = isCurrentUser
            ? $"Мои желания ({wishesResult.Value.Count})"
            : $"Желания ({wishesResult.Value.Count})";
        
        var wishListsResult = await wishServiceClient.GetUserWishListsAsync(userId, cancellationToken);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText = isCurrentUser
            ? $"Мои списки желаний ({wishListsResult.Value.Count})"
            : $"Списки желаний ({wishListsResult.Value.Count})";
        WishLists = wishListsResult.Value;

        if (isCurrentUser)
        {
            var promisedWishesResult = await wishServiceClient.GetPromisedWishesAsync(cancellationToken);
            if (promisedWishesResult.IsFailed)
            {
                return promisedWishesResult.ToResult();
            }

            PromisedWishesButtonDisplayText = $"Мои обещания ({promisedWishesResult.Value.Count})";
        }

        return Result.Ok();
    }

    [RelayCommand]
    private void NavigateToProfile(User user)
    {
        navigationService.NavigateTo<ProfilePage>(user.Id);
    }
    
    [RelayCommand]
    private void NavigateToWishesPage()
    {
        navigationService.NavigateTo<UserWishesPage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        navigationService.NavigateTo<UserPromisedWishesPage>(userId);
    }
    
    [RelayCommand]
    private void ShowWishListCard(WishList wishList)
    {
        overlayService.Show<WishListCard>(wishList.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }
    
    [RelayCommand]
    private void CreateWishList()
    {
        overlayService.Show<CreateWishListForm>();
    }
}