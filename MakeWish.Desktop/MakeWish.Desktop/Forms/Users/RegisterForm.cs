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

internal sealed partial class RegisterForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IUserServiceClient userServiceClient)
    : OverlayBase
{
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _surname = string.Empty;

    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
    }

    [RelayCommand]
    private void Register(PasswordBox passwordBox)
    {
        asyncExecutor.Execute(async token =>
        {
            var result = await RegisterAsync(Email, passwordBox.Password, Name, Surname, token);
            if (result.IsFailed)
            {
                return result;
            }

            overlayService.Show<LoginForm>();
            return Result.Ok();
        });
        
    }
    
    [RelayCommand]
    private void Login()
    {
        overlayService.Show<LoginForm>();
    }

    private async Task<Result> RegisterAsync(string email, string password, string name, string surname, CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Password = password,
            Name = name,
            Surname = surname
        };

        var registerResult = await userServiceClient.RegisterUserAsync(registerRequest, cancellationToken);
        if (registerResult.IsFailed)
        {
            return registerResult.ToResult();
        }

        var userId = registerResult.Value.Id;
        userContext.SetUserId(userId);

        var authRequest = new AuthenticateRequest
        {
            Email = email,
            Password = password
        };

        var authResult = await userServiceClient.AuthenticateUserAsync(authRequest, cancellationToken);
        if (authResult.IsFailed)
        {
            return authResult.ToResult();
        }

        userContext.SetToken(authResult.Value);
        return Result.Ok();
    }
}