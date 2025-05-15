using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Login;
using MakeWish.Desktop.Pages.Friends.Confirmed;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Profile;

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
                DoDeleteUser();
                
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
        NavigationService.ShowMessage("Переход на страницу желаний не реализован");
        // NavigationService.NavigateTo<WishesPage>(User.Id);
    }

    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowMessage("Карточка желания не реализована");
        // NavigationService.ShowOverlay<WishCard>(wishId);
    }

    [RelayCommand]
    private void NavigateToWishLists()
    {
        NavigationService.ShowMessage("Переход на страницу списков желаний не реализован");
        // NavigationService.NavigateTo<WishListsPage>(User.Id);
    }

    [RelayCommand]
    private void ShowWishListCard(Guid wishListId)
    {
        NavigationService.ShowMessage("Карточка списка желаний не реализована");
        // NavigationService.ShowOverlay<WishListCard>(wishListId);
    }

    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () =>
        {
            ShowDeleteButton = userId == UserContext.UserId;

            var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
            if (userResult.IsFailed)
            {
                NavigationService.ShowErrors(userResult.Errors);
                return;
            }

            User = userResult.Value;

            var friendsResult = await _userServiceClient.GetConfirmedFriendshipsAsync(userId, CancellationToken.None);
            if (friendsResult.IsFailed)
            {
                NavigationService.ShowErrors(friendsResult.Errors);
                return;
            }

            FriendsButtonDisplayText = $"Друзья ({friendsResult.Value.Count})";
            Friends = friendsResult.Value
                .Select(f => f.FirstUser.Id == User.Id ? f.SecondUser : f.FirstUser)
                .Take(5)
                .ToList();

            var wishesResult = await _wishServiceClient.GetUserWishesAsync(userId, CancellationToken.None);
            if (wishesResult.IsFailed)
            {
                NavigationService.ShowErrors(wishesResult.Errors);
                return;
            }

            WishesButtonDisplayText = $"Желания ({wishesResult.Value.Count})";
            Wishes = wishesResult.Value.Take(10).ToList();

            var wishListsResult = await _wishServiceClient.GetUserWishListsAsync(userId, CancellationToken.None);
            if (wishListsResult.IsFailed)
            {
                NavigationService.ShowErrors(wishListsResult.Errors);
                return;
            }

            WishListsButtonDisplayText = $"Списки желаний ({wishListsResult.Value.Count})";
            WishLists = wishListsResult.Value.Take(3).ToList();
        });
    }

    private void DoDeleteUser()
    {
        RequestExecutor.Execute(async () =>
        {
            var result = await _userServiceClient.DeleteUserAsync(User.Id, CancellationToken.None);
            if (result.IsFailed)
            {
                NavigationService.ShowErrors(result.Errors);
            }
        });
    }
}