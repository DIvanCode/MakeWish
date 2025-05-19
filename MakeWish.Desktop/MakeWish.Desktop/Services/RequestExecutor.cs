using FluentResults;

namespace MakeWish.Desktop.Services;

public interface IRequestExecutor
{
    public void Execute(Func<Task<Result>> action);
}

public sealed class RequestExecutor(INavigationService navigationService) : IRequestExecutor
{
    public void Execute(Func<Task<Result>> action) => _ = ExecuteAsync(action);

    private async Task ExecuteAsync(Func<Task<Result>> action)
    {
        var token = navigationService.BeginLoading();
        
        try
        {
            var result = await action.Invoke();
            if (result.IsFailed)
            {
                navigationService.ShowErrors(result.Errors);
            }
        }
        catch (Exception ex)
        {
            navigationService.ShowMessage(ex.Message);
        }
        finally
        {
            navigationService.EndLoading(token);
        }
    }
}