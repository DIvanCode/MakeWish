using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishListViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;

    [ObservableProperty]
    private WishList _wishList = null!;

    [ObservableProperty]
    private ViewModelBase? _selectedViewModel;

    public WishListViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient,
        Guid id)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;

        _ = LoadWishList(id);
    }
    
    [RelayCommand]
    private void ShowMainWishList()
    {
        NavigationService.NavigateTo<WishesViewModel>();
    }
    
    [RelayCommand]
    private void ShowWishLists()
    {
        NavigationService.NavigateTo<WishListsViewModel>();
    }

    [RelayCommand]
    private void SelectWish(Guid wishId)
    {
        SelectedViewModel = NavigationService.GetViewModel<WishViewModel>(wishId);
    }
    
    private async Task LoadWishList(Guid id)
    {
        try
        {
            var result = await _wishServiceClient.GetWishListAsync(id, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }
            
            WishList = result.Value;
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
    
    private void ReloadWishListAndSelectWish(Guid wishId)
    {
        _ = LoadWishList(WishList.Id);
        SelectedViewModel = NavigationService.GetViewModel<WishViewModel>(wishId);
    }
} 