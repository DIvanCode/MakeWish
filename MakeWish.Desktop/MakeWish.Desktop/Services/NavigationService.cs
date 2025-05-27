using FluentResults;
using MakeWish.Desktop.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.Desktop.Services;

internal interface INavigationService
{
    event Action<PageBase?>? OnChangePage;
    
    void NavigateToCurrent();
    void NavigateTo<TPage>(params object[] p) where TPage : PageBase;
    
    void GoForward();
    void GoBack();
    
    void ClearHistory();
}

internal sealed class NavigationService(IServiceProvider serviceProvider, IAsyncExecutor asyncExecutor)
    : INavigationService
{
    private readonly List<PageBase> _pages = [];
    private int _currentPage = -1;
    private Action? _navigateAction;
    
    public event Action<PageBase?>? OnChangePage;
    
    public void NavigateToCurrent()
    {
        OnChangePage?.Invoke(_currentPage == -1 ? null : _pages[_currentPage]);
    }
    
    public void NavigateTo<TPage>(params object[] p) where TPage : PageBase
    {
        var page = ActivatorUtilities.CreateInstance<TPage>(serviceProvider, p);
        asyncExecutor.Execute(async cancellationToken =>
        {
            var result = await page.LoadDataAsync(cancellationToken);
            if (result.IsFailed)
            {
                return result;
            }
            
            while (_currentPage + 1 < _pages.Count)
            {
                _pages.RemoveAt(_pages.Count - 1);
            }

            _pages.Add(page);
            _currentPage++;

            NavigateToCurrent();
            return Result.Ok();
        });
    }

    public void ConfirmNavigate()
    {
        _navigateAction?.Invoke();
        _navigateAction = null;
    }

    public void GoForward()
    {
        if (_currentPage < _pages.Count - 1)
        {
            _currentPage++;
        }

        NavigateToCurrent();
    }

    public void GoBack()
    {
        if (_currentPage > 0)
        {
            _currentPage--;
        }

        NavigateToCurrent();
    }

    public void ClearHistory()
    {
        _pages.Clear();
        _currentPage = -1;
        NavigateToCurrent();
    }
}