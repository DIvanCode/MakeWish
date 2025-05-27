using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Wishes;

internal sealed partial class WishCard(
    INavigationService navigationService,
    IOverlayService overlayService,
    IWishServiceClient wishServiceClient,
    Guid id)
    : OverlayBase
{
    [ObservableProperty]
    private Wish _wish = null!;
    
    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var wishResult = await wishServiceClient.GetWishAsync(id, cancellationToken);
        if (wishResult.IsFailed)
        {
            return wishResult.ToResult();
        }
            
        Wish = wishResult.Value;
        return Result.Ok();
    }
    
    [RelayCommand]
    private void NavigateToWish()
    {
        navigationService.NavigateTo<WishPage>(id);
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }
}