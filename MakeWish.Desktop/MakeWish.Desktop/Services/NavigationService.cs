using System.Windows.Media.Animation;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Dialogs.YesNo;
using Microsoft.Extensions.DependencyInjection;
using OkDialog = MakeWish.Desktop.Dialogs.Ok.OkDialog;

namespace MakeWish.Desktop.Services;

public interface INavigationService
{
    event Action? OnContentLoadingBegin;
    event Action? OnContentLoadingEnd;
    event Action<Page>? OnCurrentPageChanged;
    event Action<Overlay>? OnCurrentOverlayChanged;

    string BeginLoading();
    void EndLoading(string token);
    void CancelLoading();
    
    void NavigateTo<TPage>(params object[] p) where TPage : Page;
    void ShowOverlay<TOverlay>(params object[] p) where TOverlay : Overlay;
    void CloseLastOverlay();

    void ShowMessage(string message);
    void ShowErrors(List<IError> error);
    void ShowYesNoDialog(string message, Action onYesCommand, Action? onNoCommand = null);
    
    void GoBack();
    void GoForward();
    void ClearHistory();
}

public sealed class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly List<Page> _pagesHistory = [];
    private readonly List<Overlay> _overlays = [];
    private int _pagesHistoryPtr = -1;
    private bool _isContentLoading;
    private Action? _loadContentAction;
    private string _loadingContentToken = string.Empty;
    private readonly object _syncObject = new();
    
    public event Action? OnContentLoadingBegin;
    public event Action? OnContentLoadingEnd;
    public event Action<Page>? OnCurrentPageChanged;
    public event Action<Overlay>? OnCurrentOverlayChanged;

    public string BeginLoading()
    {
        var token = Guid.NewGuid().ToString();
        lock (_syncObject)
        {
            _loadingContentToken = token;
            _isContentLoading = true;
            OnContentLoadingBegin?.Invoke();   
        }

        return token;
    }
    
    public void EndLoading(string token)
    {
        lock (_syncObject)
        {
            if (_loadingContentToken != token)
            {
                return;
            }
            
            OnContentLoadingEnd?.Invoke();
            _loadContentAction?.Invoke();
            _isContentLoading = false;   
        }
    }

    public void CancelLoading()
    {
        lock (_syncObject)
        {
            _loadingContentToken = string.Empty;
            OnContentLoadingEnd?.Invoke();
            _loadContentAction = null;
            LoadContent();
            _isContentLoading = false;   
        }
    }
    
    public void NavigateTo<TPage>(params object[] p) where TPage : Page
    {
        var page = ActivatorUtilities.CreateInstance<TPage>(serviceProvider, p);

        _loadContentAction = () =>
        {
            while (_pagesHistoryPtr + 1 < _pagesHistory.Count)
            {
                _pagesHistory.RemoveAt(_pagesHistory.Count - 1);
            }

            _pagesHistory.Add(page);
            _pagesHistoryPtr++;

            _overlays.Clear();
            LoadContent();
        };
        
        if (!_isContentLoading)
        {
            _loadContentAction?.Invoke();
        }
    }

    public void ShowOverlay<TOverlay>(params object[] p) where TOverlay : Overlay
    {
        var overlay = ActivatorUtilities.CreateInstance<TOverlay>(serviceProvider, p);

        _loadContentAction = () =>
        {
            _overlays.Add(overlay);
            
            LoadContent();
        };
        
        if (!_isContentLoading)
        {
            _loadContentAction?.Invoke();
        }
    }

    public void ShowMessage(string message)
    {
        var okDialog = new OkDialog(message);
        okDialog.OnOkCommand += CloseLastOverlay;
        
        _overlays.Add(okDialog);

        LoadContent();
    }

    public void ShowErrors(List<IError> errors)
    {
        ShowMessage(string.Join("\n", errors.Select(error => error.Message)));
    }

    public void ShowYesNoDialog(string message, Action onYesCommand, Action? onNoCommand = null)
    {
        var yesNoDialog = new YesNoDialog(message);
        yesNoDialog.OnYesCommand += CloseLastOverlay;
        yesNoDialog.OnYesCommand += onYesCommand;
        yesNoDialog.OnNoCommand += onNoCommand ?? CloseLastOverlay;
        
        _overlays.Add(yesNoDialog);

        LoadContent();
    }

    public void CloseLastOverlay()
    {
        _overlays.RemoveAt(_overlays.Count - 1);
        
        LoadContent();
    }

    public void GoBack()
    {
        if (_pagesHistoryPtr == 0)
        {
            return;
        }

        _pagesHistoryPtr--;
        LoadCurrentPage();
    }

    public void GoForward()
    {
        if (_pagesHistoryPtr == _pagesHistory.Count - 1)
        {
            return;
        }
        
        _pagesHistoryPtr++;
        LoadCurrentPage();
    }

    public void ClearHistory()
    {
        _pagesHistory.Clear();
        _pagesHistoryPtr = -1;

        _overlays.Clear();
    }

    private void LoadContent()
    {
        if (_overlays.Count > 0)
        {
            LoadCurrentOverlay();
        }
        else
        {
            LoadCurrentPage();
        }
    }

    private void LoadCurrentPage()
    {
        _overlays.Clear();
        OnCurrentPageChanged?.Invoke(_pagesHistory[_pagesHistoryPtr]);
    }

    private void LoadCurrentOverlay()
    {
        OnCurrentOverlayChanged?.Invoke(_overlays.Last());
    }
}