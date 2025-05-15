using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Abstract;

public abstract class Card : Overlay
{
    protected readonly INavigationService NavigationService;
    protected readonly IRequestExecutor RequestExecutor;

    protected Card(INavigationService navigationService, IRequestExecutor requestExecutor)
    {
        NavigationService = navigationService;
        RequestExecutor = requestExecutor;
    }
}