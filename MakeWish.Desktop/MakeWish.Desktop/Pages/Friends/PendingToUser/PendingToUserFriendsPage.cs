using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.UserCard;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.SearchUser;
using MakeWish.Desktop.Pages.Friends.Confirmed;
using MakeWish.Desktop.Pages.Friends.PendingFromUser;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Friends.PendingToUser;

public sealed partial class PendingToUserFriendsPage : Page
{
    private readonly IUserServiceClient _userServiceClient;

    [ObservableProperty]
    private User _user = null!;

    [ObservableProperty]
    private bool _showUserDisplayName;

    [ObservableProperty]
    private bool _showPendingFriends;

    [ObservableProperty]
    private bool _showFriendsManageButtons;
    
    [ObservableProperty]
    private string _confirmedFriendsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private string _pendingFriendsToUserButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private string _pendingFriendsFromUserButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<User> _pendingFriendsToUser = [];

    public PendingToUserFriendsPage(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserContext userContext,
        IUserServiceClient userServiceClient,
        Guid userId)
        : base(navigationService, requestExecutor, userContext)
    {
        _userServiceClient = userServiceClient;

        LoadData(userId);
    }

    [RelayCommand]
    private void NavigateToProfile(Guid userId)
    {
        NavigationService.NavigateTo<ProfilePage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToConfirmedFriends()
    {
        NavigationService.NavigateTo<ConfirmedFriendsPage>(User.Id);
    }
    
    [RelayCommand]
    private void NavigateToPendingToUserFriends()
    {
        LoadData(User.Id);
    }    
    
    [RelayCommand]
    private void NavigateToPendingFromUserFriends()
    {
        NavigationService.NavigateTo<PendingFromUserFriendsPage>(User.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }

    [RelayCommand]
    private void ShowSearchUserForm()
    {
        NavigationService.ShowOverlay<SearchUserForm>();
        ((SearchUserForm)NavigationService.CurrentOverlay!).OnPickUser += userId =>
        {
            DoAddFriend(userId);
            NavigationService.CloseLastOverlay();
        };
    }
    
    [RelayCommand]
    private void ConfirmFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно принять заявку в друзья?",
            onYesCommand: () => DoConfirmFriend(userId));
    }
    
    [RelayCommand]
    private void RejectFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отклонить заявку в друзья?",
            onYesCommand: () => DoRemoveFriend(userId));
    }
    
    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }

    private async Task LoadDataAsync(Guid userId)
    {
        ShowUserDisplayName = userId != UserContext.UserId;
        ShowPendingFriends = userId == UserContext.UserId;
        ShowFriendsManageButtons = userId == UserContext.UserId;
        
        var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
        if (userResult.IsFailed)
        {
            NavigationService.ShowErrors(userResult.Errors);
            return;
        }

        User = userResult.Value;

        var confirmedFriendsResult =
            await _userServiceClient.GetConfirmedFriendshipsAsync(userId, CancellationToken.None);
        if (confirmedFriendsResult.IsFailed)
        {
            NavigationService.ShowErrors(confirmedFriendsResult.Errors);
            return;
        }

        ConfirmedFriendsButtonDisplayText = $"Список друзей ({confirmedFriendsResult.Value.Count})";

        var pendingFriendsToUserResult =
            await _userServiceClient.GetPendingFriendshipsToUserAsync(userId, CancellationToken.None);
        if (pendingFriendsToUserResult.IsFailed)
        {
            NavigationService.ShowErrors(pendingFriendsToUserResult.Errors);
            return;
        }

        PendingFriendsToUserButtonDisplayText = $"Входящие заявки ({pendingFriendsToUserResult.Value.Count})";
        PendingFriendsToUser = pendingFriendsToUserResult.Value
            .Select(f => f.FirstUser)
            .ToList();
        
        var pendingFriendsFromUserResult =
            await _userServiceClient.GetPendingFriendshipsFromUserAsync(userId, CancellationToken.None);
        if (pendingFriendsFromUserResult.IsFailed)
        {
            NavigationService.ShowErrors(pendingFriendsFromUserResult.Errors);
            return;
        }

        PendingFriendsFromUserButtonDisplayText = $"Исходящие заявки ({pendingFriendsFromUserResult.Value.Count})";
    }
    
    private void DoConfirmFriend(Guid userId)
    {
        RequestExecutor.Execute(async () =>
        {
            var request = new ConfirmFriendshipRequest
            {
                FirstUser = userId,
                SecondUser = User.Id
            };

            var result = await _userServiceClient.ConfirmFriendshipAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                NavigationService.ShowErrors(result.Errors);
            }
            
            await LoadDataAsync(User.Id);
        });
    }

    private void DoAddFriend(Guid userId)
    {
        RequestExecutor.Execute(async () =>
        {
            var request = new CreateFriendshipRequest
            {
                FirstUser = User.Id,
                SecondUser = userId
            };
            
            var result = await _userServiceClient.CreateFriendshipAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                NavigationService.ShowErrors(result.Errors);
            }

            await LoadDataAsync(User.Id);
        });
    }

    private void DoRemoveFriend(Guid userId)
    {
        RequestExecutor.Execute(async () =>
        {
            var request = new RemoveFriendshipRequest
            {
                FirstUser = User.Id,
                SecondUser = userId
            };

            var result = await _userServiceClient.RemoveFriendshipAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                NavigationService.ShowErrors(result.Errors);
            }

            await LoadDataAsync(User.Id);
        });
    }
}