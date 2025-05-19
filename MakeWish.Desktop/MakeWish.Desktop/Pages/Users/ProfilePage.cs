using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;
using LoginForm = MakeWish.Desktop.Forms.Users.LoginForm;
using UserWishesPage = MakeWish.Desktop.Pages.Wishes.UserWishesPage;

namespace MakeWish.Desktop.Pages.Users;

public sealed partial class ProfilePage : Page
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IWishServiceClient _wishServiceClient;
    
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

    public ProfilePage(
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
        if (userId == User.Id)
        {
            LoadData(User.Id);   
        }
        else
        {
            NavigationService.NavigateTo<ProfilePage>(userId);
        }
    }

    [RelayCommand]
    private void DeleteUser()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить свой аккаунт?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async() => await DeleteAsync(User.Id));
                NavigationService.ClearHistory();
                NavigationService.ShowOverlay<LoginForm>();
            });
    }

    [RelayCommand]
    private void NavigateToFriends()
    {
        NavigationService.NavigateTo<ConfirmedFriendsPage>(User.Id);
    }

    [RelayCommand]
    private void NavigateToWishes()
    {
        NavigationService.NavigateTo<UserWishesPage>(User.Id);
    }

    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowOverlay<WishCard>(wishId);
    }

    [RelayCommand]
    private void NavigateToWishLists()
    {
        NavigationService.NavigateTo<UserWishListsPage>(User.Id);
    }

    [RelayCommand]
    private void ShowWishListCard(Guid wishListId)
    {
        NavigationService.ShowOverlay<WishListCard>(wishListId);
    }

    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }
    
    private async Task<Result> LoadDataAsync(Guid userId)
    {
        ShowDeleteButton = userId == UserContext.UserId;

        var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;

        var friendsResult = await _userServiceClient.GetConfirmedFriendshipsAsync(userId, CancellationToken.None);
        if (friendsResult.IsFailed)
        {
            return friendsResult.ToResult();
        }

        FriendsButtonDisplayText = $"Друзья ({friendsResult.Value.Count})";
        Friends = friendsResult.Value
            .Select(f => f.FirstUser.Id == User.Id ? f.SecondUser : f.FirstUser)
            .Take(5)
            .ToList();

        var wishesResult = await _wishServiceClient.GetUserWishesAsync(userId, CancellationToken.None);
        if (wishesResult.IsFailed)
        {
            return wishesResult.ToResult();
        }

        WishesButtonDisplayText = $"Желания ({wishesResult.Value.Count})";
        Wishes = wishesResult.Value.Take(10).ToList();

        var wishListsResult = await _wishServiceClient.GetUserWishListsAsync(userId, CancellationToken.None);
        if (wishListsResult.IsFailed)
        {
            return wishListsResult.ToResult();
        }

        WishListsButtonDisplayText = $"Списки желаний ({wishListsResult.Value.Count})";
        WishLists = wishListsResult.Value.Take(3).ToList();
        return Result.Ok();
    }

    private async Task<Result> DeleteAsync(Guid userId)
    {
        return await _userServiceClient.DeleteUserAsync(userId, CancellationToken.None);
    }
}