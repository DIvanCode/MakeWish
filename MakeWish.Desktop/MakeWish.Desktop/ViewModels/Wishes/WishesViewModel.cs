using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishesViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;

    [ObservableProperty]
    private ObservableCollection<Wish> _wishes = [];

    [ObservableProperty]
    private ViewModelBase? _selectedViewModel;

    public WishesViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;
        ShowMainWishList();
    }
    
    [RelayCommand]
    private void ShowMainWishList()
    {
        _ = LoadMainWishList();
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
    
    [RelayCommand]
    private void ShowCreateWishForm()
    {
        SelectedViewModel = NavigationService.GetViewModel<CreateWishViewModel>((Action<Guid>)ReloadWishListAndSelectWish);
    }
    
    private async Task LoadMainWishList()
    {
        try
        {
            var result = await _wishServiceClient.GetMainWishListForUserAsync(, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            Wishes = new ObservableCollection<Wish>(result.Value.Wishes);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
    
    private void ReloadWishListAndSelectWish(Guid wishId)
    {
        _ = LoadMainWishList();
        SelectedViewModel = NavigationService.GetViewModel<WishViewModel>(wishId);
    }
} 