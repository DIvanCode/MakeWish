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

public sealed partial class SearchWishForm : Form
{
    public event Action<Wish>? OnPickWish;
    
    private readonly IWishServiceClient _wishServiceClient;
    private readonly IUserContext _userContext;
    
    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private List<Wish> _wishes = [];

    public SearchWishForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient,
        IUserContext userContext)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;
        _userContext = userContext;
    }
    
    [RelayCommand]
    private void ShowWishCard(Guid wishId)
    {
        NavigationService.ShowOverlay<WishCard>(wishId);
    }
    
    [RelayCommand]
    private void PickWish(Wish wish)
    {
        OnPickWish?.Invoke(wish);
    }
    
    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }

    [RelayCommand]
    private void SearchWish()
    {
        RequestExecutor.Execute(async () => await SearchWishAsync(Query));
    }

    private async Task<Result> SearchWishAsync(string query)
    {
        var wishesResult = await _wishServiceClient.SearchWishAsync(_userContext.UserId!.Value, query, CancellationToken.None);
        if (wishesResult.IsFailed)
        {
            return wishesResult.ToResult();
        }
            
        Wishes = wishesResult.Value;
        return Result.Ok();
    }
}