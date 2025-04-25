using CommunityToolkit.Mvvm.ComponentModel;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.ViewModels;

public abstract class ViewModelBase(INavigationService navigationService, IDialogService dialogService)
    : ObservableObject
{
    protected readonly INavigationService NavigationService = navigationService;
    protected readonly IDialogService DialogService = dialogService;
}