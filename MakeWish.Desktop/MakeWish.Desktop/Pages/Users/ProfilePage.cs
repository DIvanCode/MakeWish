using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Users;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Users;

internal sealed partial class ProfilePage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IDialogService dialogService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IUserServiceClient userServiceClient,
    IWishServiceClient wishServiceClient,
    Guid id)
    : PageBase
{
    [ObservableProperty]
    private User _user = null!;

    [ObservableProperty]
    private bool _showDeleteButton;

    [ObservableProperty] 
    private string _friendsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<User> _friends = [];

    [ObservableProperty] 
    private string _wishesButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<Wish> _wishes = [];

    [ObservableProperty] 
    private string _wishListsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<WishList> _wishLists = [];

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        ShowDeleteButton = id == userContext.UserId;

        var userResult = await userServiceClient.GetUserAsync(id, cancellationToken);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;

        var friendsResult = await userServiceClient.GetConfirmedFriendshipsAsync(id, cancellationToken);
        if (friendsResult.IsFailed)
        {
            return friendsResult.ToResult();
        }

        FriendsButtonDisplayText = $"Друзья ({friendsResult.Value.Count})";
        Friends = friendsResult.Value
            .Select(f => f.FirstUser.Id == id ? f.SecondUser : f.FirstUser)
            .Take(5)
            .ToList();

        var wishesResult = await wishServiceClient.GetUserWishesAsync(id, cancellationToken);
        if (wishesResult.IsFailed)
        {
            return wishesResult.ToResult();
        }

        WishesButtonDisplayText = $"Желания ({wishesResult.Value.Count})";
        Wishes = wishesResult.Value.Take(10).ToList();

        var wishListsResult = await wishServiceClient.GetUserWishListsAsync(id, cancellationToken);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText = $"Списки желаний ({wishListsResult.Value.Count})";
        WishLists = wishListsResult.Value.Take(3).ToList();
        return Result.Ok();
    }
    
    [RelayCommand]
    private void NavigateToProfile(User user)
    {
        if (user.Id == id)
        {
            asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
        }
        else
        {
            navigationService.NavigateTo<ProfilePage>(user.Id);
        }
    }

    [RelayCommand]
    private void DeleteUser()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить свой аккаунт?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken => await userServiceClient.DeleteUserAsync(id, cancellationToken));
                navigationService.ClearHistory();
                overlayService.Show<LoginForm>();
            });
    }

    [RelayCommand]
    private void NavigateToFriends()
    {
        navigationService.NavigateTo<ConfirmedFriendsPage>(id);
    }

    [RelayCommand]
    private void NavigateToWishes()
    {
        navigationService.NavigateTo<UserWishesPage>(id);
    }

    [RelayCommand]
    private void ShowWishCard(Wish wish)
    {
        overlayService.Show<WishCard>(wish.Id);
    }

    [RelayCommand]
    private void NavigateToWishLists()
    {
        navigationService.NavigateTo<UserWishListsPage>(id);
    }

    [RelayCommand]
    private void ShowWishListCard(WishList wishList)
    {
        overlayService.Show<WishListCard>(wishList.Id);
    }
}