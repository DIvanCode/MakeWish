using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Wishes;

internal sealed partial class SearchWishForm(
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IWishServiceClient wishServiceClient,
    IUserContext userContext)
    : OverlayBase
{
    public event Action<Wish>? OnPickWish;
    
    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private List<Wish> _wishes = [];
    
    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
    }

    [RelayCommand]
    private void ShowWishCard(Wish wish)
    {
        overlayService.Show<WishCard>(wish.Id);
    }
    
    [RelayCommand]
    private void PickWish(Wish wish)
    {
        OnPickWish?.Invoke(wish);
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }

    [RelayCommand]
    private void SearchWish()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var result = await wishServiceClient.SearchWishAsync(userContext.UserId!.Value, Query, cancellationToken);
            if (result.IsFailed)
            {
                return result.ToResult();
            }
            
            Wishes = result.Value;
            return Result.Ok();
        });
    }
}