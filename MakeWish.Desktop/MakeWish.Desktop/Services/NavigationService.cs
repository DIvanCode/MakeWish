using Microsoft.Extensions.DependencyInjection;
using MakeWish.Desktop.ViewModels;

namespace MakeWish.Desktop.Services;

public class NavigationService(IServiceProvider serviceProvider) : INavigationService
{
    private readonly List<ViewModelBase> _viewModelsHistory = [];
    private int _viewModelsHistoryPtr = -1;
    
    public event Action<ViewModelBase>? CurrentViewModelChanged;
    
    public void NavigateTo<T>(params object[] p) where T : ViewModelBase
    {
        while (_viewModelsHistoryPtr + 1 < _viewModelsHistory.Count)
        {
            _viewModelsHistory.RemoveAt(_viewModelsHistory.Count - 1);
        }

        var newViewModel = GetViewModel<T>(p);
        _viewModelsHistory.Add(newViewModel);
        _viewModelsHistoryPtr++;
        
        ReloadCurrentViewModel();
    }

    public T GetViewModel<T>(params object[] p) where T : ViewModelBase
    {
        return ActivatorUtilities.CreateInstance<T>(serviceProvider, p);
    }

    public void GoBack()
    {
        if (_viewModelsHistoryPtr == 0)
        {
            return;
        }

        _viewModelsHistoryPtr--;
        ReloadCurrentViewModel();
    }

    public void GoForward()
    {
        if (_viewModelsHistoryPtr == _viewModelsHistory.Count - 1)
        {
            return;
        }
        
        _viewModelsHistoryPtr++;
        ReloadCurrentViewModel();
    }

    public void ClearHistory()
    {
        _viewModelsHistory.Clear();
        _viewModelsHistoryPtr = -1;
    }

    private void ReloadCurrentViewModel()
    {
        CurrentViewModelChanged?.Invoke(_viewModelsHistory[_viewModelsHistoryPtr]);
    }
}