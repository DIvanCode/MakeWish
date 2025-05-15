using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Forms.Register;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Login;

public sealed partial class LoginForm : Form
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;
    
    [ObservableProperty]
    private string _email = string.Empty;

    public LoginForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
        : base(navigationService, requestExecutor)
    {
        _userServiceClient = userServiceClient;
        _userContext = userContext;
    }

    [RelayCommand]
    private void Login(PasswordBox passwordBox)
    {
        RequestExecutor.Execute(async () =>
        {
            var registerRequest = new AuthenticateRequest
            {
                Email = Email,
                Password = passwordBox.Password
            };

            var authResult = await _userServiceClient.AuthenticateUserAsync(registerRequest, CancellationToken.None);
            if (authResult.IsFailed)
            {
                NavigationService.ShowErrors(authResult.Errors);
                return;
            }

            _userContext.SetToken(authResult.Value);

            var userResult = await _userServiceClient.GetCurrentUserAsync(CancellationToken.None);
            if (userResult.IsFailed)
            {
                NavigationService.ShowErrors(userResult.Errors);
                return;
            }

            var userId = userResult.Value.Id;
            _userContext.SetUserId(userId);

            NavigationService.NavigateTo<ProfilePage>(userId);
        });
    }

    [RelayCommand]
    private void Register()
    {
        NavigationService.ShowOverlay<RegisterForm>();
    }
}