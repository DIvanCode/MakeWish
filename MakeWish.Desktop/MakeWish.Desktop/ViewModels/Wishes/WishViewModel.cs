using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.ViewModels.Users;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;
    private readonly Guid _wishId;
    
    // [ObservableProperty]
    // private string _title = string.Empty;
    //
    // [ObservableProperty]
    // private string _description = string.Empty;
    //
    // [ObservableProperty]
    // private User _owner = null!;
    //
    // [ObservableProperty]
    // private WishStatus _status = WishStatus.Created;
    //
    // [ObservableProperty]
    // private User? _promiser;
    //
    // [ObservableProperty]
    // private User? _completer;

    [ObservableProperty]
    private Wish _wish = null!;

    public WishViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient,
        Guid wishId)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;
        _wishId = wishId;
        
        _ = LoadWishDataAsync();
    }
    
    [RelayCommand]
    private void NavigateToUser(Guid userId)
    {
        NavigationService.NavigateTo<UserViewModel>(userId);
    }

    private async Task LoadWishDataAsync()
    {
        try
        {
            var result = await _wishServiceClient.GetWishAsync(_wishId, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            Wish = result.Value;
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
}