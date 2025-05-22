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

public sealed partial class WishListPage : Page
{
    private readonly IWishServiceClient _wishServiceClient;
    
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
    
    public WishListPage(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserContext userContext,
        IWishServiceClient wishServiceClient,
        Guid wishListId)
        : base(navigationService, requestExecutor, userContext)
    {
        _wishServiceClient = wishServiceClient;
        
        LoadData(wishListId);
    }
    
    [RelayCommand]
    private void ReloadWishList()
    {
        LoadData(WishList.Id);
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowOverlay<WishCard>(wishId);
    }

    [RelayCommand]
    private void Edit()
    {
        NavigationService.ShowOverlay<EditWishListForm>(WishList.Id);
    }
    
    [RelayCommand]
    private void Delete()
    {
        NavigationService.ShowYesNoDialog(
            message: "Вы действительно хотите удалить список желаний?",
            onYesCommand: () =>
            {
                RequestExecutor.Execute(async () => await DeleteAsync(WishList.Id));
                NavigationService.GoBack();
            });
    }
    
    private void LoadData(Guid wishId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(wishId));
    }

    private async Task<Result> LoadDataAsync(Guid wishListId)
    {
        var wishListResult = await _wishServiceClient.GetWishListAsync(wishListId, CancellationToken.None);
        if (wishListResult.IsFailed)
        {
            return wishListResult.ToResult();
        }

        WishList = wishListResult.Value;

        var usersWithAccessResult =
            await _wishServiceClient.GetUsersWithAccessToWishListAsync(wishListId, CancellationToken.None);
        if (usersWithAccessResult.IsFailed)
        {
            return usersWithAccessResult.ToResult();
        }
        
        UsersWithAccess = usersWithAccessResult.Value;
        
        ShowEditButton = WishList.Owner.Id == UserContext.UserId;
        ShowDeleteButton = WishList.Owner.Id == UserContext.UserId;
        ShowUsersWithAccess = WishList.Owner.Id == UserContext.UserId;

        return Result.Ok();
    }

    private async Task<Result> DeleteAsync(Guid wishListId)
    {
        return await _wishServiceClient.DeleteWishListAsync(wishListId, CancellationToken.None);
    }
}