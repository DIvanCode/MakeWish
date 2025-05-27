using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Wishes;

internal sealed partial class WishListCard(
    INavigationService navigationService,
    IOverlayService overlayService,
    IWishServiceClient wishServiceClient,
    Guid id)
    : OverlayBase
{
    [ObservableProperty]
    private WishList _wishList = null!;
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var wishListResult = await wishServiceClient.GetWishListAsync(id, cancellationToken);
        if (wishListResult.IsFailed)
        {
            return wishListResult.ToResult();
        }

        WishList = wishListResult.Value;
        WishList.Wishes = wishListResult.Value.Wishes.Take(5).ToList();
        return Result.Ok();
    }
    
    [RelayCommand]
    private void NavigateToWishList()
    {
        navigationService.NavigateTo<WishListPage>(id);
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }
    
    [RelayCommand]
    private void ShowWishCard(Wish wish)
    {
        overlayService.Show<WishCard>(wish.Id);
    }
}