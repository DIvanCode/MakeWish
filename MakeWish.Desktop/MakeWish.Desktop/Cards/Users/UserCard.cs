using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Users;

internal sealed partial class UserCard(
    INavigationService navigationService,
    IOverlayService overlayService,
    IUserServiceClient userServiceClient,
    Guid id)
    : OverlayBase
{
    [ObservableProperty]
    private User _user = null!;
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var userResult = await userServiceClient.GetUserAsync(id, cancellationToken);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }
            
        User = userResult.Value;
        return Result.Ok();
    }
    
    [RelayCommand]
    private void NavigateToProfile()
    {
        navigationService.NavigateTo<ProfilePage>(id);
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }
}