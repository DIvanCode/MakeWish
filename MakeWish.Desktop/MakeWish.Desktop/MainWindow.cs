using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Forms.Users;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop;

internal partial class MainWindow : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly ILoadingService _loadingService;
    private readonly IOverlayService _overlayService;
    private readonly IDialogService _dialogService;
    private readonly IUserContext _userContext;
    private bool _prevShowHeader;
    private Action? OpenContent;

    [ObservableProperty]
    private bool _showHeader;
    
    [ObservableProperty]
    private bool _showPage;
    
    [ObservableProperty]
    private bool _showOverlay;
    
    [ObservableProperty]
    private bool _showDialog;
    
    [ObservableProperty]
    private PageBase? _currentPage;
    
    [ObservableProperty]
    private OverlayBase? _currentOverlay;
    
    [ObservableProperty]
    private DialogBase? _currentDialog;

    [ObservableProperty]
    private bool _isContentLoading;
    
    public MainWindow(
        INavigationService navigationService, 
        ILoadingService loadingService, 
        IOverlayService overlayService, 
        IDialogService dialogService,
        IUserContext userContext)
    {
        _navigationService = navigationService;
        _loadingService = loadingService;
        _overlayService = overlayService;
        _dialogService = dialogService;
        _userContext = userContext;
        
        _navigationService.OnChangePage += ChangePage;
        _overlayService.OnChangeOverlay += ChangeOverlay;
        _dialogService.OnChangeDialog += ChangeDialog;

        _loadingService.OnBeginLoading += () =>
        {
            _prevShowHeader = ShowHeader;
            ShowHeader = false;
            IsContentLoading = true;
        };
        _loadingService.OnEndLoading += () => IsContentLoading = false;
        _loadingService.OnCancelLoading += () =>
        {
            
            ShowHeader = _prevShowHeader;
        };

        _overlayService.Show<LoginForm>();
    }
    
    [RelayCommand]
    private void CancelLoading()
    {
        _loadingService.CancelLoading();
    }

    [RelayCommand]
    private void ShowProfile()
    {
        _navigationService.NavigateTo<ProfilePage>(_userContext.UserId!);
    }

    [RelayCommand]
    private void ShowFriends()
    {
        _navigationService.NavigateTo<ConfirmedFriendsPage>(_userContext.UserId!);
    }
    
    [RelayCommand]
    private void ShowWishes()
    {
        _navigationService.NavigateTo<UserWishesPage>(_userContext.UserId!);
    }
    
    [RelayCommand]
    private void Logout()
    {
        _dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите выйти из аккаунта?",
            onYesCommand: () =>
            {
                _navigationService.ClearHistory();
                _overlayService.Show<LoginForm>();
            });
    }
    
    [RelayCommand]
    private void GoForward()
    {
        _navigationService.GoForward();
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoBack();
    }
    
    private void ChangePage(PageBase? page)
    {
        OpenContent = () =>
        {
            CurrentPage = page;
            ShowPage = page is not null;

            if (page is null)
            {
                return;
            }

            ShowHeader = true;

            CurrentOverlay = null;
            ShowOverlay = false;

            CurrentDialog = null;
            ShowDialog = false;

            _overlayService.Clear();
        };

        OpenContent.Invoke();
    }
    
    private void ChangeOverlay(OverlayBase? overlay)
    {
        OpenContent = () =>
        {
            CurrentOverlay = overlay;
            ShowOverlay = overlay is not null;

            if (overlay is null)
            {
                _navigationService.NavigateToCurrent();
                return;
            }
            
            ShowHeader = false;

            CurrentPage = null;
            ShowPage = false;

            CurrentDialog = null;
            ShowDialog = false;
        };

        OpenContent.Invoke();
    }
    
    private void ChangeDialog(DialogBase? dialog)
    {
        CurrentDialog = dialog;
        ShowDialog = dialog is not null;

        if (dialog is null)
        {
            OpenContent?.Invoke();
            return;
        }
            
        ShowHeader = false;

        CurrentPage = null;
        ShowPage = false;

        CurrentOverlay = null;
        ShowOverlay = false;
    }
}