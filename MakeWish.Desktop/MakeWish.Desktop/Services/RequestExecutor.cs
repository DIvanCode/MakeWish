namespace MakeWish.Desktop.Services;

public interface IRequestExecutor
{
    public void Execute(Func<Task> action);
}

public sealed class RequestExecutor(INavigationService navigationService) : IRequestExecutor
{
    public void Execute(Func<Task> action) => _ = ExecuteAsync(action);

    private async Task ExecuteAsync(Func<Task> action)
    {
        var token = navigationService.BeginLoading();
        
        try
        {
            await action.Invoke();
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