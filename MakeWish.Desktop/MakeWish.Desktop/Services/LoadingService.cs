namespace MakeWish.Desktop.Services;

internal interface ILoadingService
{
    event Action? OnBeginLoading;
    event Action? OnEndLoading;

    CancellationToken BeginLoading();
    void EndLoading(CancellationToken token);
    void CancelLoading();
}

internal sealed class LoadingService : ILoadingService
{
    private CancellationTokenSource? _loadingContentTokenSource;

    public event Action? OnBeginLoading;
    public event Action? OnEndLoading;
    
    public CancellationToken BeginLoading()
    {
        _loadingContentTokenSource = new CancellationTokenSource();
        
        OnBeginLoading?.Invoke();
        
        return _loadingContentTokenSource.Token;
    }
    
    public void EndLoading(CancellationToken token)
    {
        if (_loadingContentTokenSource?.Token != token)
        {
            return;
        }
        
        _loadingContentTokenSource = null;
        
        OnEndLoading?.Invoke();
    }

    public void CancelLoading()
    {
        _loadingContentTokenSource?.Cancel();
    }
}