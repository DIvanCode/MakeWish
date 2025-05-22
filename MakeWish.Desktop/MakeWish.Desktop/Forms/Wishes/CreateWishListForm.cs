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

public sealed partial class CreateWishListForm : Form
{
    private readonly IWishServiceClient _wishServiceClient;
    
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private List<Wish> _wishes = [];
    
    public CreateWishListForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IWishServiceClient wishServiceClient)
        : base(navigationService, requestExecutor)
    {
        _wishServiceClient = wishServiceClient;
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
    
    [RelayCommand]
    private void ShowSearchWishForm()
    {
        NavigationService.ShowOverlay<SearchWishForm>();
        ((SearchWishForm)NavigationService.CurrentOverlay!).OnPickWish += wish =>
        {
            Wishes.Add(wish);
            NavigationService.CloseLastOverlay();
        };
    }

    [RelayCommand]
    private void Create()
    {
        RequestExecutor.Execute(async () =>
        {
            var result = await CreateAsync(Title, Wishes);
            if (result.IsFailed)
            {
                return result.ToResult();
            }

            NavigationService.NavigateTo<WishListPage>(result.Value);
            return Result.Ok();
        });
    }

    private async Task<Result<Guid>> CreateAsync(string title, List<Wish> wishes)
    {
        var createRequest = new CreateWishListRequest
        {
            Title = title
        };
        
        var createResult = await _wishServiceClient.CreateWishListAsync(createRequest, CancellationToken.None);
        if (createResult.IsFailed)
        {
            return createResult.ToResult();
        }

        var wishListId = createResult.Value.Id;

        var errors = new List<IError>();
        foreach (var wish in wishes)
        {
            var addResult = await _wishServiceClient.AddWishToWishListAsync(wishListId, wish.Id, CancellationToken.None);
            if (addResult.IsFailed)
            {
                errors.AddRange(addResult.Errors);
            }
        }

        return errors.Count != 0 ? Result.Fail(errors) : Result.Ok(wishListId);
    }
}