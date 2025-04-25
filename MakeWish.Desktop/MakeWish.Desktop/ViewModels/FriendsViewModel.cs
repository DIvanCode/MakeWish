using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.Clients.UserContext;

namespace MakeWish.Desktop.ViewModels;

public partial class FriendsViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;

    [ObservableProperty]
    private ObservableCollection<User> _users = [];

    public FriendsViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
        : base(navigationService, dialogService)
    {
        _userServiceClient = userServiceClient;
        _userContext = userContext;

        ShowConfirmedFriendships();
    }

    [RelayCommand]
    private void ShowConfirmedFriendships()
    {
        _ = LoadConfirmedFriendshipsAsync();
    }

    [RelayCommand]
    private void ShowPendingFriendshipsFromUser()
    {
        NavigationService.NavigateTo<FriendsPendingFromUserViewModel>();
    }

    [RelayCommand]
    private void ShowPendingFriendshipsToUser()
    {
        NavigationService.NavigateTo<FriendsPendingToUserViewModel>();
    }
    
    [RelayCommand]
    private async Task RemoveFriendship(Guid userId)
    {
        try
        {
            var request = new RemoveFriendshipRequest
            {
                FirstUser = userId,
                SecondUser = _userContext.UserId!.Value
            };
            
            var result = await _userServiceClient.RemoveFriendshipAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            ShowConfirmedFriendships();
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private void NavigateToUser(Guid userId)
    {
        NavigationService.NavigateTo<UserViewModel>(userId);
    }
    
    private async Task LoadConfirmedFriendshipsAsync()
    {
        try
        {
            var result = await _userServiceClient.GetConfirmedFriendshipsAsync(_userContext.UserId!.Value, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            Users = new ObservableCollection<User>(result.Value.Select(f =>
                f.FirstUser.Id == _userContext.UserId!.Value ? f.SecondUser : f.FirstUser));
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
} 