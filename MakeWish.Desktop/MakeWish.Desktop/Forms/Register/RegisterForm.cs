using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Forms.Login;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Register;

public sealed partial class RegisterForm : Form
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IUserContext _userContext;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _surname = string.Empty;

    public RegisterForm(
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
    private void Register(PasswordBox passwordBox)
    {
        RequestExecutor.Execute(async () =>
        {
            var registerRequest = new RegisterRequest
            {
                Email = Email,
                Password = passwordBox.Password,
                Name = Name,
                Surname = Surname
            };

            var registerResult = await _userServiceClient.RegisterUserAsync(registerRequest, CancellationToken.None);
            if (registerResult.IsFailed)
            {
                NavigationService.ShowErrors(registerResult.Errors);
                return;
            }

            var userId = registerResult.Value.Id;
            _userContext.SetUserId(userId);

            var authRequest = new AuthenticateRequest
            {
                Email = Email,
                Password = passwordBox.Password
            };

            var authResult = await _userServiceClient.AuthenticateUserAsync(authRequest, CancellationToken.None);
            if (authResult.IsFailed)
            {
                NavigationService.ShowErrors(authResult.Errors);
                return;
            }

            _userContext.SetToken(authResult.Value);

            NavigationService.NavigateTo<ProfilePage>(userId);
        });
    }
    
    [RelayCommand]
    private void Login()
    {
        NavigationService.ShowOverlay<LoginForm>();
    }
}