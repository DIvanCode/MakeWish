using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

public sealed partial class WishPage : Page
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private Wish _wish = null!;
    
    [ObservableProperty]
    private bool _showEditButton;
    
    [ObservableProperty]
    private bool _showDeleteButton;
    
    [ObservableProperty]
    private bool _showRestoreButton;
    
    [ObservableProperty]
    private bool _showPromiseButton;
    
    [ObservableProperty]
    private bool _showCompleteButton;
    
    [ObservableProperty]
    private bool _showPromiseCancelButton;
    
    [ObservableProperty]
    private bool _showCompleteApproveButton;
    
    [ObservableProperty]
    private bool _showCompleteRejectButton;
    
    public WishPage(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserContext userContext,
        IWishServiceClient wishServiceClient,
        Guid wishId)
        : base(navigationService, requestExecutor, userContext)
    {
        _wishServiceClient = wishServiceClient;

        LoadData(wishId);
    }
    
    [RelayCommand]
    private void ReloadWish()
    {
        LoadData(Wish.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }

    [RelayCommand]
    private void Edit()
    {
        NavigationService.ShowMessage("Форма редактирования желания не реализована");
        // NavigationService.ShowOverlay<EditWishForm>(Wish.Id);
    }
    
    [RelayCommand]
    private void Delete()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить желание?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await DeleteAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void Restore()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите восстановить желание?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await RestoreAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void Promise()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите обещать исполнение желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await PromiseAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void Complete()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите исполнили желание?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void PromiseCancel()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отказаться от исполнения желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await PromiseCancelAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void CompleteApprove()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите подтвердить исполнение желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteApproveAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }
    
    [RelayCommand]
    private void CompleteReject()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите отклонить исполнение желания?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await CompleteRejectAsync(Wish.Id));
                LoadData(Wish.Id);
            });
    }

    private void LoadData(Guid wishId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(wishId));
    }

    private async Task<Result> LoadDataAsync(Guid wishId)
    {
        var wishResult = await _wishServiceClient.GetWishAsync(wishId, CancellationToken.None);
        if (wishResult.IsFailed)
        {
            return wishResult.ToResult();
        }

        Wish = wishResult.Value;

        var wishIsCreated = Wish.Status is WishStatus.Created;
        var wishIsDeleted = Wish.Status is WishStatus.Deleted;
        var wishIsPromised = Wish.Status is WishStatus.Promised;
        var wishIsCompleted = Wish.Status is WishStatus.Completed;
        
        var userIsOwner = Wish.Owner.Id == UserContext.UserId;
        var userIsPromiser = Wish.Promiser?.Id == UserContext.UserId;
        
        ShowEditButton = wishIsCreated && userIsOwner;
        ShowDeleteButton = wishIsCreated && userIsOwner;
        ShowRestoreButton = wishIsDeleted && userIsOwner;
        ShowPromiseButton = wishIsCreated;
        ShowCompleteButton = (wishIsCreated && userIsOwner) || (wishIsPromised && userIsPromiser);
        ShowPromiseCancelButton = wishIsPromised && userIsPromiser;
        ShowCompleteApproveButton = wishIsCompleted && userIsOwner;
        ShowCompleteRejectButton = wishIsCompleted && userIsOwner;

        return Result.Ok();
    }
    
    private async Task<Result> DeleteAsync(Guid wishId)
    {
        var result = await _wishServiceClient.DeleteWishAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }

    private async Task<Result> RestoreAsync(Guid wishId)
    {
        var result = await _wishServiceClient.RestoreWishAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> PromiseAsync(Guid wishId)
    {
        var result = await _wishServiceClient.PromiseAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> CompleteAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> PromiseCancelAsync(Guid wishId)
    {
        var result = await _wishServiceClient.PromiseCancelAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> CompleteApproveAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteApproveAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
    
    private async Task<Result> CompleteRejectAsync(Guid wishId)
    {
        var result = await _wishServiceClient.CompleteRejectAsync(wishId, CancellationToken.None);
        return result.IsFailed ? result.ToResult() : Result.Ok();
    }
}