using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.Clients.UserContext;

namespace MakeWish.Desktop.ViewModels;

public partial class RegisterViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _surname = string.Empty;

    public RegisterViewModel(
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
    private async Task Register(PasswordBox passwordBox)
    {
        try
        {
            var request = new RegisterRequest
            {
                Email = Email,
                Password = passwordBox.Password,
                Name = Name,
                Surname = Surname
            };

            var registerResult = await _userServiceClient.RegisterUserAsync(request, CancellationToken.None);
            if (registerResult.IsFailed)
            {
                var error = string.Join("\n", registerResult.Errors.Select(e => e.Message));
                DialogService.ShowError(error);
                return;
            }

            var authRequest = new AuthenticateRequest
            {
                Email = Email,
                Password = passwordBox.Password
            };

            var authResult = await _userServiceClient.AuthenticateUserAsync(authRequest, CancellationToken.None);
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
    private void NavigateToLogin()
    {
        NavigationService.NavigateTo<LoginViewModel>();
    }
} 