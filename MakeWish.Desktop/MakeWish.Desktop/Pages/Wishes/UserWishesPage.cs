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

internal sealed partial class UserWishesPage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IDialogService dialogService,
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
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var isCurrentUser = userId == userContext.UserId;
        
        ShowUserDisplayName = !isCurrentUser;
        ShowPromisedWishesButton = isCurrentUser;
        ShowCreateWishButton = isCurrentUser;
        
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

        List<Wish> wishes;
        if (isCurrentUser)
        {
            wishes = wishesResult.Value
                .Where(wish => wish.Status is not WishStatus.Completed)
                .ToList();
            WaitingApproveWishes = wishesResult.Value
                .Where(wish => wish.Status is WishStatus.Completed)
                .ToList();
        }
        else
        {
            wishes = wishesResult.Value;
        }
        
        wishes.Sort((wish1, wish2) =>
        {
            if (wish1.StatusOrder != wish2.StatusOrder)
            {
                return wish1.StatusOrder < wish2.StatusOrder ? -1 : 1;
            }
            
            return string.Compare(wish1.Title, wish2.Title, StringComparison.Ordinal);
        });

        Wishes = wishes;

        var wishListsResult = await wishServiceClient.GetUserWishListsAsync(userId, cancellationToken);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText = isCurrentUser
            ? $"Мои списки желаний ({wishListsResult.Value.Count})"
            : $"Списки желаний ({wishListsResult.Value.Count})";

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
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void NavigateToWishListsPage()
    {
        navigationService.NavigateTo<UserWishListsPage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToPromisedWishesPage()
    {
        navigationService.NavigateTo<UserPromisedWishesPage>(userId);
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
    private void CompleteApprove(Wish wish)
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите подтвердить исполнение желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteApproveAsync(wish.Id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void CompleteReject(Wish wish)
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите отклолнить исполнение желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteRejectAsync(wish.Id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void CreateWish()
    {
        overlayService.Show<CreateWishForm>();
    }
}