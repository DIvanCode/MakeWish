using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class CreateWishListViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;

    [ObservableProperty]
    private string _title = string.Empty;

    public CreateWishListViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;
    }

    [RelayCommand]
    private async Task Create()
    {
        try
        {
            var request = new CreateWishListRequest
            {
                Title = Title,
            };

            var result = await _wishServiceClient.CreateWishListAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                var error = string.Join("\n", result.Errors.Select(e => e.Message));
                DialogService.ShowError(error);
            }
            
            NavigationService.NavigateTo<WishListViewModel>(result.Value.Id);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
} 