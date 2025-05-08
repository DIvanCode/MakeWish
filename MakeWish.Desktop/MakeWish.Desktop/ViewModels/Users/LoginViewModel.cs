using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Users;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;

    [ObservableProperty]
    private string _email = string.Empty;

    public LoginViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
        : base(navigationService, dialogService)
    {
        _userServiceClient = userServiceClient;
        _userContext = userContext;
    }

    [RelayCommand]
    private async Task Login(PasswordBox passwordBox)
    {
        try
        {
            var request = new AuthenticateRequest
            {
                Email = Email,
                Password = passwordBox.Password
            };

            var authResult = await _userServiceClient.AuthenticateUserAsync(request, CancellationToken.None);
            if (authResult.IsFailed)
            {
                var error = string.Join("\n", authResult.Errors.Select(e => e.Message));
                DialogService.ShowError(error);
                return;
            }

            _userContext.SetToken(authResult.Value);

            var userResult = await _userServiceClient.GetCurrentUserAsync(CancellationToken.None);
            if (userResult.IsFailed)
            {
                var error = string.Join("\n", userResult.Errors.Select(e => e.Message));
                DialogService.ShowError(error);
                return;
            }

            var userId = userResult.Value.Id;
            _userContext.SetUserId(userId);

            NavigationService.ClearHistory();
            NavigationService.NavigateTo<UserViewModel>(userId);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private void NavigateToRegister()
    {
        NavigationService.NavigateTo<RegisterViewModel>();
    }
} 