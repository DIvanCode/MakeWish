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
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

public sealed partial class UserPromisedWishesPage : Page
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private User _user = null!;
    
    [ObservableProperty]
    private string _wishesButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private string _wishListsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private string _promisedWishesButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<Wish> _promisedWishes = [];
    
    public UserPromisedWishesPage(
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
    private void NavigateToWishesPage()
    {
        NavigationService.NavigateTo<UserWishesPage>(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        NavigationService.NavigateTo<UserWishListsPage>(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        LoadData(User.Id);
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
    private void Complete(Guid wishId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно исполнили желание?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteAsync(wishId));
                LoadData(User.Id);
            });
    }
    
    [RelayCommand]
    private void PromiseCancel(Guid wishId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отказаться от исполнения желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await PromiseCancelAsync(wishId));
                LoadData(User.Id);
            });
    }
    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }
    
    private async Task<Result> LoadDataAsync(Guid userId)
    {
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

        WishesButtonDisplayText = $"Мои желания ({wishesResult.Value.Count})";
        
        var wishListsResult = await _wishServiceClient.GetUserWishListsAsync(userId, CancellationToken.None);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText =  $"Мои списки желаний ({wishListsResult.Value.Count})";
        
        var promisedWishesResult = await _wishServiceClient.GetPromisedWishesAsync(CancellationToken.None);
        if (promisedWishesResult.IsFailed)
        {
            return promisedWishesResult.ToResult();
        }

        PromisedWishesButtonDisplayText = $"Мои обещания ({promisedWishesResult.Value.Count})";
        PromisedWishes = promisedWishesResult.Value;
        
        PromisedWishes.Sort((wish1, wish2) =>
        {
            if (wish1.Status == wish2.Status)
            {
                return string.Compare(wish1.Title, wish2.Title, StringComparison.Ordinal);
            }

            if (wish1.Status is WishStatus.Promised) return -1;
            if (wish2.Status is WishStatus.Promised) return 1;

            if (wish1.Status is WishStatus.Completed) return -1;
            if (wish2.Status is WishStatus.Completed) return 1;

            if (wish1.Status is WishStatus.Approved) return -1;
            return 1;
        });
        
        return Result.Ok();
    }
    
    private async Task<Result> CompleteAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> PromiseCancelAsync(Guid wishId)
    {
        var result = await _wishServiceClient.PromiseCancelAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
}