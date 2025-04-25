using MakeWish.Desktop.ViewModels;

namespace MakeWish.Desktop.Services;

public interface INavigationService
{
    event Action<ViewModelBase>? CurrentViewModelChanged;
    void NavigateTo<T>(params object[] p) where T : ViewModelBase;
    void GoBack();
    void GoForward();
    void ClearHistory();
}