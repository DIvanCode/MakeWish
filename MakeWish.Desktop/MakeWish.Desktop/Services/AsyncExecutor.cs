using FluentResults;

namespace MakeWish.Desktop.Services;

internal interface IAsyncExecutor
{
    public void Execute(Func<CancellationToken, Task<Result>> action);
}

internal sealed class AsyncExecutor(ILoadingService loadingService, IDialogService dialogService) : IAsyncExecutor
{
    public void Execute(Func<CancellationToken, Task<Result>> action) => _ = ExecuteAsync(action);
    
    private async Task ExecuteAsync(Func<CancellationToken, Task<Result>> action)
    {
        var token = loadingService.BeginLoading();
        try
        {
            var result = await action.Invoke(token);
            if (result.IsFailed)
            {
                loadingService.CancelLoading();
                loadingService.EndLoading(token);
                dialogService.ShowErrors(result.Errors);
            }
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                dialogService.ShowMessage(ex.Message);
            }
            
            loadingService.CancelLoading();
        }
        finally
        {
            loadingService.EndLoading(token);
        }
    }
}