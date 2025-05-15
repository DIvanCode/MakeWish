using CommunityToolkit.Mvvm.ComponentModel;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Abstract;

public abstract class Page : ObservableObject
{
    protected readonly INavigationService NavigationService;
    protected readonly IRequestExecutor RequestExecutor;
    protected readonly IUserContext UserContext;

    protected Page(INavigationService navigationService, IRequestExecutor requestExecutor, IUserContext userContext)
    {
        NavigationService = navigationService;
        RequestExecutor = requestExecutor;
        UserContext = userContext;
    }
}