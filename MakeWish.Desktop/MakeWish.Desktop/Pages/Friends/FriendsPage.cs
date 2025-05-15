using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.UserCard;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Friends;

public sealed partial class FriendsPage : Page
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
    private bool _isConfirmedFriendsSelected;
    
    [ObservableProperty]
    private bool _isPendingFriendsToUserSelected;
    
    [ObservableProperty]
    private bool _isPendingFriendsFromUserSelected;
    
    [ObservableProperty]
    private string _confirmedFriendsButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<User> _confirmedFriends = [];
    
    [ObservableProperty]
    private string _pendingFriendsToUserButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<User> _pendingFriendsToUser = [];
    
    [ObservableProperty]
    private string _pendingFriendsFromUserButtonDisplayText = string.Empty;
    
    [ObservableProperty]
    private List<User> _pendingFriendsFromUser = [];

    public FriendsPage(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserContext userContext,
        IUserServiceClient userServiceClient,
        Guid userId)
        : base(navigationService, requestExecutor, userContext)
    {
        _userServiceClient = userServiceClient;

        LoadData(userId, initialLoad: true);
        LoadConfirmedFriends(userId);
    }

    [RelayCommand]
    private void NavigateToProfile(Guid userId)
    {
        NavigationService.NavigateTo<ProfilePage>(userId);
    }
    
    [RelayCommand]
    private void SelectConfirmedFriends()
    {
        LoadConfirmedFriends(User.Id);
    }
    
    [RelayCommand]
    private void SelectPendingFriendsToUser()
    {
        LoadPendingFriendsToUser(User.Id);
    }    
    
    [RelayCommand]
    private void SelectPendingFriendsFromUser()
    {
        LoadPendingFriendsFromUser(User.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void RemoveFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите убрать из друзей этого пользователя?",
            onYesCommand: () => DoRemoveFriend(userId));
    }
    
    [RelayCommand]
    private void ConfirmFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно принять заявку в друзья?",
            onYesCommand: () => DoConfirmFriend(userId));
    }
    
    [RelayCommand]
    private void CancelFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отменить заявку в друзья?",
            onYesCommand: () => DoRemoveFriend(userId));
    }
    
    [RelayCommand]
    private void RejectFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отклонить заявку в друзья?",
            onYesCommand: () => DoRemoveFriend(userId));
    }
    
    private void LoadConfirmedFriends(Guid userId)
    {
        ShowUserDisplayName = userId != UserContext.UserId;
        ShowPendingFriends = userId == UserContext.UserId;
        ShowFriendsManageButtons = userId == UserContext.UserId;

        IsConfirmedFriendsSelected = true;
        IsPendingFriendsToUserSelected = false;
        IsPendingFriendsFromUserSelected = false;

        LoadData(userId);
    }
    
    private void LoadPendingFriendsToUser(Guid userId)
    {
        ShowUserDisplayName = userId != UserContext.UserId;
        ShowPendingFriends = userId == UserContext.UserId;
        ShowFriendsManageButtons = userId == UserContext.UserId;

        IsConfirmedFriendsSelected = false;
        IsPendingFriendsToUserSelected = true;
        IsPendingFriendsFromUserSelected = false;

        LoadData(userId);
    }
    
    private void LoadPendingFriendsFromUser(Guid userId)
    {
        ShowUserDisplayName = userId != UserContext.UserId;
        ShowPendingFriends = userId == UserContext.UserId;
        ShowFriendsManageButtons = userId == UserContext.UserId;

        IsConfirmedFriendsSelected = false;
        IsPendingFriendsToUserSelected = false;
        IsPendingFriendsFromUserSelected = true;

        LoadData(userId);
    }
    
    private void LoadData(Guid userId, bool initialLoad = false)
    {
        RequestExecutor.Execute(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            
            if (initialLoad)
            {
                var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
                if (userResult.IsFailed)
                {
                    NavigationService.ShowErrors(userResult.Errors);
                    return;
                }

                User = userResult.Value;
            }

            var confirmedFriendsResult =
                await _userServiceClient.GetConfirmedFriendshipsAsync(userId, CancellationToken.None);
            if (confirmedFriendsResult.IsFailed)
            {
                NavigationService.ShowErrors(confirmedFriendsResult.Errors);
                return;
            }

            ConfirmedFriendsButtonDisplayText = $"Список друзей ({confirmedFriendsResult.Value.Count})";
            ConfirmedFriends = confirmedFriendsResult.Value
                .Select(f => f.FirstUser.Id == userId ? f.SecondUser : f.FirstUser)
                .ToList();

            if (ShowPendingFriends)
            {
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
            }

            if (ShowPendingFriends)
            {
                var pendingFriendsFromUserResult =
                    await _userServiceClient.GetPendingFriendshipsFromUserAsync(userId, CancellationToken.None);
                if (pendingFriendsFromUserResult.IsFailed)
                {
                    NavigationService.ShowErrors(pendingFriendsFromUserResult.Errors);
                    return;
                }

                PendingFriendsFromUserButtonDisplayText =
                    $"Исходящие заявки ({pendingFriendsFromUserResult.Value.Count})";
                PendingFriendsFromUser = pendingFriendsFromUserResult.Value
                    .Select(f => f.SecondUser)
                    .ToList();
            }
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
        });
        
        LoadData(User.Id);
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
        });
        
        LoadData(User.Id);
    }
}