using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.UserCard;

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
        RequestExecutor.Execute(async () =>
        {
            var userResult = await _userServiceClient.GetUserAsync(userId, CancellationToken.None);
            if (userResult.IsFailed)
            {
                NavigationService.ShowErrors(userResult.Errors);
                return;
            }
            
            User = userResult.Value;
        });
    }
}