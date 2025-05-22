using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Users;

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
        ((SearchUserForm)NavigationService.CurrentOverlay!).OnPickUser += user =>
        {
            RequestExecutor.Execute(async () => await AddFriendAsync(User.Id, user.Id));
            NavigationService.CloseLastOverlay();
            LoadData(User.Id);
        };
    }
    
    [RelayCommand]
    private void ConfirmFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно принять заявку в друзья?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await ConfirmFriendAsync(userId, User.Id));
                LoadData(User.Id);
            });
    }
    
    [RelayCommand]
    private void RejectFriend(Guid userId)
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отклонить заявку в друзья?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await RemoveFriendAsync(User.Id, userId));
                LoadData(User.Id);
            });
    }
    
    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }

    private async Task<Result> LoadDataAsync(Guid userId)
    {
        ShowUserDisplayName = userId != UserContext.UserId;
        ShowPendingFriends = userId == UserContext.UserId;
        ShowFriendsManageButtons = userId == UserContext.UserId;
        
        var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;

        var confirmedFriendsResult =
            await _userServiceClient.GetConfirmedFriendshipsAsync(userId, CancellationToken.None);
        if (confirmedFriendsResult.IsFailed)
        {
            return confirmedFriendsResult.ToResult();
        }

        ConfirmedFriendsButtonDisplayText = $"Список друзей ({confirmedFriendsResult.Value.Count})";

        var pendingFriendsToUserResult =
            await _userServiceClient.GetPendingFriendshipsToUserAsync(userId, CancellationToken.None);
        if (pendingFriendsToUserResult.IsFailed)
        {
            return pendingFriendsToUserResult.ToResult();
        }

        PendingFriendsToUserButtonDisplayText = $"Входящие заявки ({pendingFriendsToUserResult.Value.Count})";
        PendingFriendsToUser = pendingFriendsToUserResult.Value
            .Select(f => f.FirstUser)
            .ToList();
        
        var pendingFriendsFromUserResult =
            await _userServiceClient.GetPendingFriendshipsFromUserAsync(userId, CancellationToken.None);
        if (pendingFriendsFromUserResult.IsFailed)
        {
            return pendingFriendsFromUserResult.ToResult();
        }

        PendingFriendsFromUserButtonDisplayText = $"Исходящие заявки ({pendingFriendsFromUserResult.Value.Count})";
        return Result.Ok();
    }

    private async Task<Result> AddFriendAsync(Guid firstUserId, Guid secondUserId)
    {
        var request = new CreateFriendshipRequest
        {
            FirstUser = firstUserId,
            SecondUser = secondUserId
        };
        
        var result = await _userServiceClient.CreateFriendshipAsync(request, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> ConfirmFriendAsync(Guid firstUserId, Guid secondUserId)
    {
        var request = new ConfirmFriendshipRequest
        {
            FirstUser = firstUserId,
            SecondUser = secondUserId
        };
        
        var result = await _userServiceClient.ConfirmFriendshipAsync(request, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }

    private async Task<Result> RemoveFriendAsync(Guid firstUserId, Guid secondUserId)
    {
        var request = new RemoveFriendshipRequest
        {
            FirstUser = firstUserId,
            SecondUser = secondUserId
        };

        return await _userServiceClient.RemoveFriendshipAsync(request, CancellationToken.None);
    }
}