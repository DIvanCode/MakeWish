using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.ViewModels.Users;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishesViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;

    [ObservableProperty]
    private ObservableCollection<Wish> _wishes = [];

    [ObservableProperty]
    private WishViewModel? _selectedWishViewModel;

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
        SelectedWishViewModel = NavigationService.GetViewModel<WishViewModel>(wishId);
    }
    
    private async Task LoadMainWishList()
    {
        try
        {
            var result = await _wishServiceClient.GetMainWishListAsync(CancellationToken.None);
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
} 