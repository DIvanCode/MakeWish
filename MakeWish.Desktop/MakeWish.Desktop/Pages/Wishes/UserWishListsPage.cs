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

public sealed partial class UserWishListsPage : Page
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IWishServiceClient _wishServiceClient;
    
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
    
    public UserWishListsPage(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserContext userContext,
        IUserServiceClient userServiceClient,
        IWishServiceClient wishServiceClient,
        Guid userId)
        : base(navigationService, requestExecutor, userContext)
    {
        _userServiceClient = userServiceClient;
        _wishServiceClient = wishServiceClient;
        
        LoadData(userId);
    }
    
    [RelayCommand]
    private void NavigateToProfile(Guid userId)
    {
        NavigationService.NavigateTo<ProfilePage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToWishesPage()
    {
        NavigationService.NavigateTo<UserWishesPage>(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        LoadData(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        NavigationService.NavigateTo<UserPromisedWishesPage>(User.Id);
    }
    
    [RelayCommand]
    private void ShowWishListCard(Guid wishListId)
    {
        NavigationService.ShowOverlay<WishListCard>(wishListId);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void CreateWishList()
    {
        NavigationService.ShowOverlay<CreateWishListForm>();
    }
    
    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }
    
    private async Task<Result> LoadDataAsync(Guid userId)
    {
        var isCurrentUser = userId == UserContext.UserId;
        
        ShowUserDisplayName = !isCurrentUser;
        ShowPromisedWishesButton = isCurrentUser;
        ShowCreateWishListButton = isCurrentUser;
        
        var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;
        
        var wishesResult = await _wishServiceClient.GetUserWishesAsync(userId, CancellationToken.None);
        if (wishesResult.IsFailed)
        {
            return wishesResult.ToResult();
        }

        WishesButtonDisplayText = isCurrentUser
            ? $"Мои желания ({wishesResult.Value.Count})"
            : $"Желания ({wishesResult.Value.Count})";
        
        var wishListsResult = await _wishServiceClient.GetUserWishListsAsync(userId, CancellationToken.None);
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
            var promisedWishesResult = await _wishServiceClient.GetPromisedWishesAsync(CancellationToken.None);
            if (promisedWishesResult.IsFailed)
            {
                return promisedWishesResult.ToResult();
            }

            PromisedWishesButtonDisplayText = $"Мои обещания ({promisedWishesResult.Value.Count})";
        }

        return Result.Ok();
    }
}