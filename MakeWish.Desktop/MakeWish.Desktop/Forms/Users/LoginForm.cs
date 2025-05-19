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
            var result = await LoginAsync(Email, passwordBox.Password);
            if (result.IsFailed)
            {
                return result;
            }
            
            NavigationService.NavigateTo<ProfilePage>(_userContext.UserId!.Value);
            return Result.Ok();
        });
    }

    [RelayCommand]
    private void Register()
    {
        NavigationService.ShowOverlay<RegisterForm>();
    }

    private async Task<Result> LoginAsync(string email, string password)
    {
        var registerRequest = new AuthenticateRequest
        {
            Email = email,
            Password = password
        };

        var authResult = await _userServiceClient.AuthenticateUserAsync(registerRequest, CancellationToken.None);
        if (authResult.IsFailed)
        {
            return authResult.ToResult();
        }

        _userContext.SetToken(authResult.Value);

        var userResult = await _userServiceClient.GetCurrentUserAsync(CancellationToken.None);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        var userId = userResult.Value.Id;
        _userContext.SetUserId(userId);

        return Result.Ok();
    }
}