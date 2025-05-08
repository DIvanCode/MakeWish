using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.ViewModels.Users;

namespace MakeWish.Desktop.ViewModels.Wishes;

public partial class WishViewModel : ViewModelBase
{
    private readonly IWishServiceClient _wishServiceClient;
    private readonly IUserContext _userContext;
    private readonly Guid _wishId;

    [ObservableProperty]
    private Wish _wish = null!;

    [ObservableProperty]
    private bool _showPromiser;
    
    [ObservableProperty]
    private bool _showCompleter;
    
    [ObservableProperty]
    private bool _showApproveButton;
    
    [ObservableProperty]
    private bool _showRejectButton;
    
    [ObservableProperty]
    private bool _showWaitingApprove;

    public WishViewModel(
        INavigationService navigationService,
        IDialogService dialogService,
        IWishServiceClient wishServiceClient,
        IUserContext userContext,
        Guid wishId)
        : base(navigationService, dialogService)
    {
        _wishServiceClient = wishServiceClient;
        _userContext = userContext;
        _wishId = wishId;
        
        _ = LoadWishDataAsync();
    }
    
    [RelayCommand]
    private void NavigateToUser(Guid userId)
    {
        NavigationService.NavigateTo<UserViewModel>(userId);
    }
    
    [RelayCommand]
    private async Task Approve()
    {
        try
        {
            var result = await _wishServiceClient.CompleteApproveAsync(_wishId, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }
            
            await LoadWishDataAsync(result);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
    
    [RelayCommand]
    private async Task Reject()
    {
        try
        {
            var result = await _wishServiceClient.CompleteRejectAsync(_wishId, CancellationToken.None);
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }
            
            await LoadWishDataAsync(result);
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
    
    private async Task LoadWishDataAsync(Result<Wish>? result = null)
    {
        try
        {
            result ??= await _wishServiceClient.GetWishAsync(_wishId, CancellationToken.None); 
            if (result.IsFailed)
            {
                DialogService.ShowError(string.Join("\n", result.Errors.Select(e => e.Message)));
                return;
            }

            Wish = result.Value;
            ShowPromiser = Wish.Status is WishStatus.Promised;
            ShowCompleter = Wish.Status is WishStatus.Completed or WishStatus.Approved;
            ShowApproveButton = Wish.Status == WishStatus.Completed && Wish.Owner.Id == _userContext.UserId;
            ShowRejectButton = Wish.Status == WishStatus.Completed && Wish.Owner.Id == _userContext.UserId;
            ShowWaitingApprove = Wish.Status == WishStatus.Completed && Wish.Completer?.Id == _userContext.UserId;
        }
        catch (Exception ex)
        {
            DialogService.ShowError(ex.Message);
        }
    }
}