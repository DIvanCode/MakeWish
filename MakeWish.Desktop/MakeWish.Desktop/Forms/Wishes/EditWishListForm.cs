using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Forms.Users;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Wishes;

public sealed partial class EditWishListForm : Form
{
    private enum WishListEventType
    {
        AddWish = 0,
        RemoveWish = 1,
        AllowAccess = 2,
        DenyAccess = 3
    }
    
    private readonly IWishServiceClient _wishServiceClient;
    private readonly List<(WishListEventType, Wish)> _wishListWishesEvents = [];
    private readonly List<(WishListEventType, User)> _wishListUsersAccessEvents = [];
    
    [ObservableProperty]
    private Guid _id = Guid.Empty;
    
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Wish> _wishes = [];
    
    [ObservableProperty]
    private ObservableCollection<User> _usersWithAccess = [];
    
    public EditWishListForm(
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
    private void RemoveWish(Guid wishId)
    {
        var wish = Wishes.Single(wish => wish.Id == wishId);
        Wishes.Remove(wish);
        _wishListWishesEvents.Add((WishListEventType.RemoveWish, wish));
    }
    
    [RelayCommand]
    private void ShowSearchWishForm()
    {
        NavigationService.ShowOverlay<SearchWishForm>();
        ((SearchWishForm)NavigationService.CurrentOverlay!).OnPickWish += wish =>
        {
            if (Wishes.All(w => w.Id != wish.Id))
            {
                Wishes.Add(wish);
                _wishListWishesEvents.Add((WishListEventType.AddWish, wish));
            }
            
            NavigationService.CloseLastOverlay();
        };
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void DenyUserAccess(Guid userId)
    {
        var user = UsersWithAccess.Single(user => user.Id == userId);
        UsersWithAccess.Remove(user);
        _wishListUsersAccessEvents.Add((WishListEventType.DenyAccess, user));
    }
    
    [RelayCommand]
    private void ShowSearchUserForm()
    {
        const bool onlyFriends = true;
        NavigationService.ShowOverlay<SearchUserForm>(onlyFriends);
        ((SearchUserForm)NavigationService.CurrentOverlay!).OnPickUser += user =>
        {
            if (UsersWithAccess.All(u => u.Id != user.Id))
            {
                UsersWithAccess.Add(user);
                _wishListUsersAccessEvents.Add((WishListEventType.AllowAccess, user));   
            }
            
            NavigationService.CloseLastOverlay();
        };
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

        Id = wishListResult.Value.Id;
        Title = wishListResult.Value.Title;
        Wishes = new ObservableCollection<Wish>(wishListResult.Value.Wishes);

        var usersWithAccessResult =
            await _wishServiceClient.GetUsersWithAccessToWishListAsync(wishListId, CancellationToken.None);
        if (usersWithAccessResult.IsFailed)
        {
            return usersWithAccessResult.ToResult();
        }
        
        UsersWithAccess = new ObservableCollection<User>(usersWithAccessResult.Value);
        
        return Result.Ok();
    }

    [RelayCommand]
    private void Save()
    {
        RequestExecutor.Execute(async () =>
        {
            var result = await SaveAsync(Id, Title, _wishListWishesEvents, _wishListUsersAccessEvents);
            if (result.IsFailed)
            {
                return result;
            }

            NavigationService.NavigateTo<WishListPage>(Id);
            return Result.Ok();
        });
    }
    
    private async Task<Result> SaveAsync(
        Guid id,
        string title,
        List<(WishListEventType, Wish)> wishListWishesEvents,
        List<(WishListEventType, User)> wishListUserAccessEvents)
    {
        var updateRequest = new UpdateWishListRequest
        {
            Id = id,
            Title = title
        };

        var updateResult = await _wishServiceClient.UpdateWishListAsync(updateRequest, CancellationToken.None);
        if (updateResult.IsFailed)
        {
            return updateResult.ToResult();
        }

        var errors = new List<IError>();
        foreach (var (eventType, wish) in wishListWishesEvents)
        {
            if (eventType is WishListEventType.AddWish)
            {
                var addResult = await _wishServiceClient.AddWishToWishListAsync(id, wish.Id, CancellationToken.None);
                if (addResult.IsFailed)
                {
                    errors.AddRange(addResult.Errors);
                }
            }
            else
            {
                var removeResult = await _wishServiceClient.RemoveWishFromWishListAsync(id, wish.Id, CancellationToken.None);
                if (removeResult.IsFailed)
                {
                    errors.AddRange(removeResult.Errors);
                }
            }
        }

        foreach (var (eventType, user) in wishListUserAccessEvents)
        {
            if (eventType is WishListEventType.AllowAccess)
            {
                var allowAccessResult = await _wishServiceClient.AllowUserAccessToWishListAsync(id, user.Id, CancellationToken.None);
                if (allowAccessResult.IsFailed)
                {
                    errors.AddRange(allowAccessResult.Errors);
                }
            }
            else
            {
                var denyAccessResult = await _wishServiceClient.DenyUserAccessToWishListAsync(id, user.Id, CancellationToken.None);
                if (denyAccessResult.IsFailed)
                {
                    errors.AddRange(denyAccessResult.Errors);
                }
            }
        }

        return errors.Count != 0 ? Result.Fail(errors) : Result.Ok();
    }
}