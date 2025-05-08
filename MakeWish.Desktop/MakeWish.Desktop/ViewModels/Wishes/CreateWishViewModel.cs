using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class CreateWishViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;
    private readonly Action<Guid> _callback;

    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string _description = string.Empty;

    public CreateWishViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient,
        Action<Guid> callback)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;
        _callback = callback;
    }

    [RelayCommand]
    private async Task Create()
    {
        try
        {
            var request = new CreateWishRequest
            {
                Title = Title,
                Description = Description
            };

            var result = await _wishServiceClient.CreateWishAsync(request, CancellationToken.None);
            if (result.IsFailed)
            {
                var error = string.Join("\n", result.Errors.Select(e => e.Message));
                DialogService.ShowError(error);
            }
            
            _callback(result.Value.Id);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
} 