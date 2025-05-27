using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Pages.Wishes;

internal sealed partial class WishListPage(
    INavigationService navigationService,
    IOverlayService overlayService,
    IDialogService dialogService,
    IAsyncExecutor asyncExecutor,
    IUserContext userContext,
    IWishServiceClient wishServiceClient,
    Guid id)
    : PageBase
{
    [ObservableProperty]
    private WishList _wishList = null!;
    
    [ObservableProperty]
    private bool _showEditButton;
    
    [ObservableProperty]
    private bool _showDeleteButton;
    
    [ObservableProperty]
    private List<User> _usersWithAccess = [];

    [ObservableProperty]
    private bool _showUsersWithAccess;

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var wishListResult = await wishServiceClient.GetWishListAsync(id, cancellationToken);
        if (wishListResult.IsFailed)
        {
            return wishListResult.ToResult();
        }

        WishList = wishListResult.Value;

        var usersWithAccessResult =
            await wishServiceClient.GetUsersWithAccessToWishListAsync(id, cancellationToken);
        if (usersWithAccessResult.IsFailed)
        {
            return usersWithAccessResult.ToResult();
        }
        
        UsersWithAccess = usersWithAccessResult.Value;
        
        ShowEditButton = WishList.Owner.Id == userContext.UserId;
        ShowDeleteButton = WishList.Owner.Id == userContext.UserId;
        ShowUsersWithAccess = WishList.Owner.Id == userContext.UserId;

        return Result.Ok();
    }

    [RelayCommand]
    private void ReloadWishList()
    {
        asyncExecutor.Execute(async cancellationToken => await LoadDataAsync(cancellationToken));
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }
    
    [RelayCommand]
    private void ShowWishCard(Wish wish)
    {
        overlayService.Show<WishCard>(wish.Id);
    }

    [RelayCommand]
    private void Edit()
    {
        overlayService.Show<EditWishListForm>(id);
    }
    
    [RelayCommand]
    private void Delete()
    {
        dialogService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить список желаний?",
            onYesCommand: () =>
            {
                asyncExecutor.Execute(async cancellationToken
                    => await wishServiceClient.DeleteWishListAsync(id, cancellationToken));
                
                navigationService.GoBack();
            });
    }
}