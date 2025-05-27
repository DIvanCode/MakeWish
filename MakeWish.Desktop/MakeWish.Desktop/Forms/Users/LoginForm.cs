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

internal sealed partial class LoginForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IUserServiceClient userServiceClient)
    : OverlayBase
{
    [ObservableProperty]
    private string _email = string.Empty;

    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
    }

    [RelayCommand]
    private void Login(PasswordBox passwordBox)
    {
        asyncExecutor.Execute(async token =>
        {
            var result = await LoginAsync(Email, passwordBox.Password, token);
            if (result.IsFailed)
            {
                return result;
            }
            
            navigationService.NavigateTo<ProfilePage>(userContext.UserId!.Value);
            return Result.Ok();
        });
    }

    [RelayCommand]
    private void Register()
    {
        overlayService.Show<RegisterForm>();
    }

    private async Task<Result> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var registerRequest = new AuthenticateRequest
        {
            Email = email,
            Password = password
        };

        var authResult = await userServiceClient.AuthenticateUserAsync(registerRequest, cancellationToken);
        if (authResult.IsFailed)
        {
            return authResult.ToResult();
        }

        userContext.SetToken(authResult.Value);

        var userResult = await userServiceClient.GetCurrentUserAsync(cancellationToken);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        var userId = userResult.Value.Id;
        userContext.SetUserId(userId);

        return Result.Ok();
    }
}