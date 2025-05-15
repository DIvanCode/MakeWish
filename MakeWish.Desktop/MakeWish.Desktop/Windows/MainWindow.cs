using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Forms.Login;
using MakeWish.Desktop.Pages.Friends.Confirmed;
using MakeWish.Desktop.Pages.Profile;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Windows;

public partial class MainWindow : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IUserContext _userContext;
    private bool _prevShowHeaderValue;

    [ObservableProperty]
    private bool _showHeader;
    
    [ObservableProperty]
    private bool _showPage;
    
    [ObservableProperty]
    private bool _showOverlay;
    
    [ObservableProperty]
    private Page? _currentPage;
    
    [ObservableProperty]
    private Overlay? _currentOverlay;

    [ObservableProperty]
    private bool _isContentLoading;
    
    public MainWindow(INavigationService navigationService, IUserContext userContext)
    {
        _navigationService = navigationService;
        _userContext = userContext;

        _navigationService.OnContentLoadingBegin += BeginLoading;
        _navigationService.OnContentLoadingEnd += EndLoading;
        _navigationService.OnCurrentPageChanged += ChangeCurrentPage;
        _navigationService.OnCurrentOverlayChanged += ChangeCurrentOverlay;

        _navigationService.ShowOverlay<LoginForm>();
    }

    private void BeginLoading()
    {
        _prevShowHeaderValue = ShowHeader;
        ShowHeader = false;
        IsContentLoading = true;
    }
    
    private void EndLoading()
    {
        ShowHeader = _prevShowHeaderValue;
        IsContentLoading = false;
    }
    
    [RelayCommand]
    private void CancelLoading()
    {
        _navigationService.CancelLoading();
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
        _navigationService.ShowMessage("Переход на страницу желаний не реализован");
        // _navigationService.NavigateTo<WishesPage>(_userContext.UserId!);
    }
    
    [RelayCommand]
    private void Logout()
    {
        _navigationService.ShowYesNoDialog(
            message: "Вы действительно хотите выйти из аккаунта?",
            onYesCommand: () =>
            {
                _navigationService.ClearHistory();
                _navigationService.ShowOverlay<LoginForm>();
            });
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
    
    private void ChangeCurrentPage(Page page)
    {
        ShowHeader = true;
        
        CurrentPage = page;
        ShowPage = true;
        
        CurrentOverlay = null;
        ShowOverlay = false;
    }
    
    private void ChangeCurrentOverlay(Overlay overlay)
    {
        ShowHeader = false;
        
        CurrentPage = null;
        ShowPage = false;
        
        CurrentOverlay = overlay;
        ShowOverlay = true;
    }
}