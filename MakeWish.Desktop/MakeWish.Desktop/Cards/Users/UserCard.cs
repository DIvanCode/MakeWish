using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Users;

public sealed partial class UserCard : Card
{
    private readonly IUserServiceClient _userServiceClient;

    [ObservableProperty]
    private User _user = null!;

    public UserCard(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserServiceClient userServiceClient,
        Guid userId)
        : base(navigationService, requestExecutor)
    {
        _userServiceClient = userServiceClient;
        
        LoadData(userId);
    }
    
    [RelayCommand]
    private void NavigateToProfile()
    {
        NavigationService.NavigateTo<ProfilePage>(User.Id);
    }
    
    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }
    
    private void LoadData(Guid userId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(userId));
    }

    private async Task<Result> LoadDataAsync(Guid userId)
    {
        var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }
            
        User = userResult.Value;
        return Result.Ok();
    }
}