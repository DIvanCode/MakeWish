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

internal sealed partial class PendingFromUserFriendsPage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IDialogService dialogService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IUserServiceClient userServiceClient,
    Guid userId)
    : PageBase
{
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
    private List<User> _pendingFriendsFromUser = [];

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        ShowUserDisplayName = userId != userContext.UserId;
        ShowPendingFriends = userId == userContext.UserId;
        ShowFriendsManageButtons = userId == userContext.UserId;
        
        var userResult = await userServiceClient.GetUserAsync(userId, cancellationToken);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        User = userResult.Value;

        var confirmedFriendsResult =
            await userServiceClient.GetConfirmedFriendshipsAsync(userId, cancellationToken);
        if (confirmedFriendsResult.IsFailed)
        {
            return confirmedFriendsResult.ToResult();
        }

        ConfirmedFriendsButtonDisplayText = $"Список друзей ({confirmedFriendsResult.Value.Count})";

        var pendingFriendsToUserResult =
            await userServiceClient.GetPendingFriendshipsToUserAsync(userId, cancellationToken);
        if (pendingFriendsToUserResult.IsFailed)
        {
            return pendingFriendsToUserResult.ToResult();
        }

        PendingFriendsToUserButtonDisplayText = $"Входящие заявки ({pendingFriendsToUserResult.Value.Count})";
        
        var pendingFriendsFromUserResult =
            await userServiceClient.GetPendingFriendshipsFromUserAsync(userId, cancellationToken);
        if (pendingFriendsFromUserResult.IsFailed)
        {
            return pendingFriendsFromUserResult.ToResult();
        }

        PendingFriendsFromUserButtonDisplayText = $"Исходящие заявки ({pendingFriendsFromUserResult.Value.Count})";
        PendingFriendsFromUser = pendingFriendsFromUserResult.Value
            .Select(f => f.SecondUser)
            .ToList();
        return Result.Ok();
    }
    
    [RelayCommand]
    private void NavigateToProfile(User user)
    {
        navigationService.NavigateTo<ProfilePage>(user.Id);
    }
    
    [RelayCommand]
    private void NavigateToConfirmedFriends()
    {
        navigationService.NavigateTo<ConfirmedFriendsPage>(userId);
    }
    
    [RelayCommand]
    private void NavigateToPendingToUserFriends()
    {
        navigationService.NavigateTo<PendingToUserFriendsPage>(userId);
    }    
    
    [RelayCommand]
    private void NavigateToPendingFromUserFriends()
    {
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }

    [RelayCommand]
    private void ShowSearchUserForm()
    {
        overlayService.Show<SearchUserForm>();
        ((SearchUserForm)overlayService.Current!).OnPickUser += user =>
        {
            asyncExecutor.Execute(async cancellationToken =>
            {
                var request = new CreateFriendshipRequest
                {
                    FirstUser = userId,
                    SecondUser = user.Id
                };
        
                var result = await userServiceClient.CreateFriendshipAsync(request, cancellationToken);
                return result.IsFailed ? result.ToResult() : Result.Ok();
            });
            
            overlayService.Close();
            navigationService.NavigateTo<PendingFromUserFriendsPage>(userId);
        };
    }
    
    [RelayCommand]
    private void CancelFriend(User user)
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите отменить заявку в друзья?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var request = new RemoveFriendshipRequest
                    {
                        FirstUser = userId,
                        SecondUser = user.Id
                    };

                    var result = await userServiceClient.RemoveFriendshipAsync(request, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result;
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
}