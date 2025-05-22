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

public sealed partial class UserWishesPage : Page
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
    private List<Wish> _wishes = [];

    [ObservableProperty]
    private List<Wish> _waitingApproveWishes = [];
    
    [ObservableProperty]
    private string _wishListsButtonDisplayText = string.Empty;

    [ObservableProperty]
    private bool _showPromisedWishesButton;
    
    [ObservableProperty]
    private string _promisedWishesButtonDisplayText = string.Empty;

    [ObservableProperty]
    private bool _showCreateWishButton;
    
    public UserWishesPage(
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
        LoadData(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        NavigationService.NavigateTo<UserWishListsPage>(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        NavigationService.NavigateTo<UserPromisedWishesPage>(User.Id);
    }
    
    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowOverlay<WishCard>(wishId);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void CompleteApprove(Guid wishId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите подтвердить исполнение желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteApproveAsync(wishId));
                LoadData(User.Id);
            });
    }
    
    [RelayCommand]
    private void CompleteReject(Guid wishId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отклолнить исполнение желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteRejectAsync(wishId));
                LoadData(User.Id);
            });
    }
    
    [RelayCommand]
    private void CreateWish()
    {
        NavigationService.ShowOverlay<CreateWishForm>();
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
        ShowCreateWishButton = isCurrentUser;
        
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

        if (isCurrentUser)
        {
            Wishes = wishesResult.Value
                .Where(wish => wish.Status is not WishStatus.Completed)
                .ToList();
            WaitingApproveWishes = wishesResult.Value
                .Where(wish => wish.Status is WishStatus.Completed)
                .ToList();
        }
        else
        {
            Wishes = wishesResult.Value;
        }
        
        Wishes.Sort((wish1, wish2) =>
        {
            if (wish1.Status == wish2.Status)
            {
                return string.Compare(wish1.Title, wish2.Title, StringComparison.Ordinal);
            }

            if (wish1.Status is WishStatus.Created) return -1;
            if (wish2.Status is WishStatus.Created) return 1;

            if (wish1.Status is WishStatus.Promised) return -1;
            if (wish2.Status is WishStatus.Promised) return 1;
            
            if (wish1.Status is WishStatus.Completed) return -1;
            if (wish2.Status is WishStatus.Completed) return 1;
            
            if (wish1.Status is WishStatus.Approved) return -1;
            if (wish2.Status is WishStatus.Approved) return 1;

            if (wish1.Status is WishStatus.Deleted) return -1;
            return 1;
        });

        var wishListsResult = await _wishServiceClient.GetUserWishListsAsync(userId, CancellationToken.None);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText = isCurrentUser
            ? $"Мои списки желаний ({wishListsResult.Value.Count})"
            : $"Списки желаний ({wishListsResult.Value.Count})";

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

    private async Task<Result> CompleteApproveAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteApproveAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> CompleteRejectAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteRejectAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
}