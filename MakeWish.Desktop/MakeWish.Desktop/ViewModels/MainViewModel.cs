using MakeWish.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.ViewModels.Users;
using MakeWish.Desktop.ViewModels.Wishes;

namespace MakeWish.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IUserContext _userContext;
    private ViewModelBase? _currentViewModel;

    [ObservableProperty]
    private bool _isHeaderVisible;

    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            IsHeaderVisible = _userContext.IsAuthenticated;
            OnPropertyChanged();
        }
    }

    public MainViewModel(
        INavigationService navigationService, 
        IDialogService dialogService,
        IUserContext userContext)
        : base(navigationService, dialogService)
    {
        _navigationService = navigationService;
        _userContext = userContext;
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;
        
        _navigationService.NavigateTo<LoginViewModel>();
    }

    [RelayCommand]
    private void NavigateToProfile()
    {
        _navigationService.NavigateTo<UserViewModel>(_userContext.UserId!.Value);
    }
    
    [RelayCommand]
    private void NavigateToFriends()
    {
        _navigationService.NavigateTo<FriendsViewModel>();
    }
    
    [RelayCommand]
    private void NavigateToWishes()
    {
        _navigationService.NavigateTo<WishesViewModel>();
    }

    [RelayCommand]
    private void Logout()
    {
        _userContext.Clear();
        _navigationService.NavigateTo<LoginViewModel>();
    }
    
    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoBack();
    }
    
    [RelayCommand]
    private void GoForward()
    {
        _navigationService.GoForward();
    }

    private void OnCurrentViewModelChanged(ViewModelBase viewModel)
    {
        CurrentViewModel = viewModel;
    }
}