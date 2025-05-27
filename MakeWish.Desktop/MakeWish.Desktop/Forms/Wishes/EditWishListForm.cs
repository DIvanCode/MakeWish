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

internal sealed partial class EditWishListForm(
    INavigationService navigationService,
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IWishServiceClient wishServiceClient,
    Guid id)
    : OverlayBase
{
    private readonly List<(WishListEventType, Wish)> _wishListWishesEvents = [];
    private readonly List<(WishListEventType, User)> _wishListUsersAccessEvents = [];
    
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Wish> _wishes = [];
    
    [ObservableProperty]
    private ObservableCollection<User> _usersWithAccess = [];

    public override async Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        var result = await wishServiceClient.GetWishListAsync(id, cancellationToken);
        if (result.IsFailed)
        {
            return result.ToResult();
        }

        Title = result.Value.Title;
        Wishes = new ObservableCollection<Wish>(result.Value.Wishes);

        var usersWithAccessResult = await wishServiceClient.GetUsersWithAccessToWishListAsync(id, cancellationToken);
        if (usersWithAccessResult.IsFailed)
        {
            return usersWithAccessResult.ToResult();
        }
        
        UsersWithAccess = new ObservableCollection<User>(usersWithAccessResult.Value);
        
        return Result.Ok();
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
    private void RemoveWish(Wish wish)
    {
        Wishes.Remove(wish);
        _wishListWishesEvents.Add((WishListEventType.RemoveWish, wish));
    }
    
    [RelayCommand]
    private void ShowSearchWishForm()
    {
        overlayService.Show<SearchWishForm>();
        ((SearchWishForm)overlayService.Current!).OnPickWish += wish =>
        {
            if (Wishes.All(w => w.Id != wish.Id))
            {
                Wishes.Add(wish);
                _wishListWishesEvents.Add((WishListEventType.AddWish, wish));
            }
            
            overlayService.Close();
        };
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }
    
    [RelayCommand]
    private void DenyUserAccess(User user)
    {
        UsersWithAccess.Remove(user);
        _wishListUsersAccessEvents.Add((WishListEventType.DenyAccess, user));
    }
    
    [RelayCommand]
    private void ShowSearchUserForm()
    {
        const bool onlyFriends = true;
        overlayService.Show<SearchUserForm>(onlyFriends);
        ((SearchUserForm)overlayService.Current!).OnPickUser += user =>
        {
            if (UsersWithAccess.All(u => u.Id != user.Id))
            {
                UsersWithAccess.Add(user);
                _wishListUsersAccessEvents.Add((WishListEventType.AllowAccess, user));   
            }
            
            overlayService.Close();
        };
    }
    
    [RelayCommand]
    private void Save()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var updateRequest = new UpdateWishListRequest
            {
                Id = id,
                Title = Title
            };

            var updateResult = await wishServiceClient.UpdateWishListAsync(updateRequest, cancellationToken);
            if (updateResult.IsFailed)
            {
                return updateResult.ToResult();
            }

            var errors = new List<IError>();
            foreach (var (eventType, wish) in _wishListWishesEvents)
            {
                if (eventType is WishListEventType.AddWish)
                {
                    var addResult = await wishServiceClient.AddWishToWishListAsync(id, wish.Id, cancellationToken);
                    if (addResult.IsFailed)
                    {
                        errors.AddRange(addResult.Errors);
                    }
                }
                else
                {
                    var removeResult = await wishServiceClient.RemoveWishFromWishListAsync(id, wish.Id, cancellationToken);
                    if (removeResult.IsFailed)
                    {
                        errors.AddRange(removeResult.Errors);
                    }
                }
            }

            foreach (var (eventType, user) in _wishListUsersAccessEvents)
            {
                if (eventType is WishListEventType.AllowAccess)
                {
                    var allowAccessResult = await wishServiceClient.AllowUserAccessToWishListAsync(id, user.Id, cancellationToken);
                    if (allowAccessResult.IsFailed)
                    {
                        errors.AddRange(allowAccessResult.Errors);
                    }
                }
                else
                {
                    var denyAccessResult = await wishServiceClient.DenyUserAccessToWishListAsync(id, user.Id, cancellationToken);
                    if (denyAccessResult.IsFailed)
                    {
                        errors.AddRange(denyAccessResult.Errors);
                    }
                }
            }

            if (errors.Count != 0)
            {
                return Result.Fail(errors);
            }

            navigationService.NavigateTo<WishListPage>(id);
            return Result.Ok();
        });
    }
    
    private enum WishListEventType
    {
        AddWish = 0,
        RemoveWish = 1,
        AllowAccess = 2,
        DenyAccess = 3
    }
}