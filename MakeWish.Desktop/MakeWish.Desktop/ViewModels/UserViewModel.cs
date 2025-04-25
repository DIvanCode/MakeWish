using CommunityToolkit.Mvvm.ComponentModel;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.Clients.UserContext;

namespace MakeWish.Desktop.ViewModels;

public partial class UserViewModel : ViewModelBase
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly Guid _userId;
    
    [ObservableProperty]
    private string _displayName = string.Empty;

    public UserViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IUserServiceClient userServiceClient,
        IUserContext userContext)
        : this(navigationService, dialogService, userServiceClient, userContext.UserId!.Value)
    {
    }

    public UserViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IUserServiceClient userServiceClient,
        Guid userId)
        : base(navigationService, dialogService)
    {
        _userServiceClient = userServiceClient;
        _userId = userId;
        
        _ = LoadUserDataAsync();
    }

    private async Task LoadUserDataAsync()
    {
        try
        {
            var result = await _userServiceClient.GetUserAsync(_userId, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            DisplayName = $"{result.Value.Name} {result.Value.Surname}";
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
}