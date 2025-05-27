using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

internal sealed partial class WishPage(
    IOverlayService overlayService,
    IDialogService dialogService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IWishServiceClient wishServiceClient,
    Guid id)
    : PageBase
{
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
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var wishResult = await wishServiceClient.GetWishAsync(id, cancellationToken);
        if (wishResult.IsFailed)
        {
            return wishResult.ToResult();
        }

        Wish = wishResult.Value;

        var wishIsCreated = Wish.Status is WishStatus.Created;
        var wishIsDeleted = Wish.Status is WishStatus.Deleted;
        var wishIsPromised = Wish.Status is WishStatus.Promised;
        var wishIsCompleted = Wish.Status is WishStatus.Completed;
        
        var userIsOwner = Wish.Owner.Id == userContext.UserId;
        var userIsPromiser = Wish.Promiser?.Id == userContext.UserId;
        
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

    [RelayCommand]
    private void ReloadWish()
    {
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }

    [RelayCommand]
    private void Edit()
    {
        overlayService.Show<EditWishForm>(id);
    }
    
    [RelayCommand]
    private void Delete()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить желание?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.DeleteWishAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void Restore()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите восстановить желание?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.RestoreWishAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void Promise()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите обещать исполнение желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.PromiseAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void Complete()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите исполнили желание?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void PromiseCancel()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите отказаться от исполнения желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.PromiseCancelAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void CompleteApprove()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите подтвердить исполнение желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteApproveAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
    
    [RelayCommand]
    private void CompleteReject()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите отклонить исполнение желания?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken =>
                {
                    var result = await wishServiceClient.CompleteRejectAsync(id, cancellationToken);
                    if (result.IsFailed)
                    {
                        return result.ToResult();
                    }

                    return await LoadDataAsync(cancellationToken);
                });
            });
    }
}