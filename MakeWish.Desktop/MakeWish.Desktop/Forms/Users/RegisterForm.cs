using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Users;

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
            var result = await RegisterAsync(Email, passwordBox.Password, Name, Surname);
            if (result.IsFailed)
            {
                return result;
            }
            
            NavigationService.NavigateTo<ProfilePage>(_userContext.UserId!.Value);
            return Result.Ok();
        });
        
    }
    
    [RelayCommand]
    private void Login()
    {
        NavigationService.ShowOverlay<LoginForm>();
    }

    private async Task<Result> RegisterAsync(string email, string password, string name, string surname)
    {
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        };

        var registerResult = await _userServiceClient.RegisterUserAsync(registerRequest, CancellationToken.None);
        if (registerResult.IsFailed)
        {
            return registerResult.ToResult();
        }

        var userId = registerResult.Value.Id;
        _userContext.SetUserId(userId);

        var authRequest = new AuthenticateRequest
        {
            Email = email,
            Password = password
        };

        var authResult = await _userServiceClient.AuthenticateUserAsync(authRequest, CancellationToken.None);
        if (authResult.IsFailed)
        {
            return authResult.ToResult();
        }

        _userContext.SetToken(authResult.Value);
        return Result.Ok();
    }
}