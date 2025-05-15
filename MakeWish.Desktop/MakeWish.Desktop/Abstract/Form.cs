using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Abstract;

public abstract class Form : Overlay
{
    protected readonly INavigationService NavigationService;
    protected readonly IRequestExecutor RequestExecutor;

    protected Form(INavigationService navigationService, IRequestExecutor requestExecutor)
    {
        NavigationService = navigationService;
        RequestExecutor = requestExecutor;
    }
}