using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Wishes;

internal sealed partial class CreateWishListForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IWishServiceClient wishServiceClient)
    : OverlayBase
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private List<Wish> _wishes = [];

    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
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
    
    [RelayCommand]
    private void ShowSearchWishForm()
    {
        overlayService.Show<SearchWishForm>();
        ((SearchWishForm)overlayService.Current!).OnPickWish += wish =>
        {
            Wishes.Add(wish);
            overlayService.Close();
        };
    }

    [RelayCommand]
    private void Create()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var createRequest = new CreateWishListRequest
            {
                Title = Title
            };
        
            var createResult = await wishServiceClient.CreateWishListAsync(createRequest, cancellationToken);
            if (createResult.IsFailed)
            {
                return createResult.ToResult();
            }

            var wishListId = createResult.Value.Id;

            var errors = new List<IError>();
            foreach (var wish in Wishes)
            {
                var addResult = await wishServiceClient.AddWishToWishListAsync(wishListId, wish.Id, cancellationToken);
                if (addResult.IsFailed)
                {
                    errors.AddRange(addResult.Errors);
                }
            }

            if (errors.Count != 0)
            {
                return Result.Fail(errors);
            }

            navigationService.NavigateTo<WishListPage>(createResult.Value.Id);
            return Result.Ok();
        });
    }
}