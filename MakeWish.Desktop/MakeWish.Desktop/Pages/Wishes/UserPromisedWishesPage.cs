using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

internal sealed partial class UserPromisedWishesPage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IDialogService dialogService,
    IAsyncExecutor asyncExecutor,
    IUserServiceClient userServiceClient,
    IWishServiceClient wishServiceClient,
    Guid userId)
    : PageBase
{
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

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
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

        WishesButtonDisplayText = $"Мои желания ({wishesResult.Value.Count})";
        
        var wishListsResult = await wishServiceClient.GetUserWishListsAsync(userId, cancellationToken);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText =  $"Мои списки желаний ({wishListsResult.Value.Count})";
        
        var promisedWishesResult = await wishServiceClient.GetPromisedWishesAsync(cancellationToken);
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
    
    [RelayCommand]
    private void NavigateToWishesPage()
    {
        navigationService.NavigateTo<UserWishesPage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        navigationService.NavigateTo<UserWishListsPage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void ShowWishCard(Wish wish)
    {
        overlayService.Show<WishCard>(wish.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }
    
    [RelayCommand]
    private void Complete(Wish wish)
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно исполнили желание?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteAsync(wish.Id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void PromiseCancel(Wish wish)
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите отказаться от исполнения желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.PromiseCancelAsync(wish.Id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
}