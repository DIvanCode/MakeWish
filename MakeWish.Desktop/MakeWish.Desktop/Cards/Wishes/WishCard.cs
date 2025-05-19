using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Cards.Wishes;

public sealed partial class WishCard : Card
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private Wish _wish = null!;
    
    public WishCard(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient,
        Guid wishId)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;
        
        LoadData(wishId);
    }
    
    [RelayCommand]
    private void NavigateToWish()
    {
        NavigationService.NavigateTo<WishPage>(Wish.Id);
    }
    
    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
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
        return Result.Ok();
    }
}