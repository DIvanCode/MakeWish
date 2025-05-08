using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishListsViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;

    [ObservableProperty]
    private ObservableCollection<WishList> _wishLists = [];

    [ObservableProperty]
    private ViewModelBase? _selectedViewModel;
    
    public WishListsViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;

        ShowWishLists();
    }

    [RelayCommand]
    private void ShowMainWishList()
    {
        NavigationService.NavigateTo<WishesViewModel>();
    }
    
    [RelayCommand]
    private void ShowWishLists()
    {
        _ = LoadWishLists();
    }
    
    [RelayCommand]
    private void ShowCreateWishListForm()
    {
        SelectedViewModel = NavigationService.GetViewModel<CreateWishListViewModel>();
    }

    [RelayCommand]
    private void NavigateToWishList(Guid id)
    {
        NavigationService.NavigateTo<WishListViewModel>(id);
    }
    
    private async Task LoadWishLists()
    {
        try
        {
            var result = await _wishServiceClient.GetWishListsAsync(CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }
            
            WishLists = new ObservableCollection<WishList>(result.Value);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
}