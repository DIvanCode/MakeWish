using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Wishes;

public sealed partial class WishListCard : Card
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private WishList _wishList = null!;
    
    public WishListCard(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient,
        Guid wishListId)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;
        
        LoadData(wishListId);
    }
    
    [RelayCommand]
    private void NavigateToWishList()
    {
        NavigationService.NavigateTo<WishListPage>(WishList.Id);
    }
    
    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }
    
    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowOverlay<WishCard>(wishId);
    }
    
    private void LoadData(Guid wishListId)
    {
        RequestExecutor.Execute(async () => await LoadDataAsync(wishListId));
    }

    private async Task<Result> LoadDataAsync(Guid wishListId)
    {
        var wishListResult = await _wishServiceClient.GetWishListAsync(wishListId, CancellationToken.None);
        if (wishListResult.IsFailed)
        {
            return wishListResult.ToResult();
        }
            
        WishList = wishListResult.Value with { Wishes = wishListResult.Value.Wishes.Take(5).ToList() };
        return Result.Ok();
    }
}