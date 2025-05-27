using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Users;

internal sealed partial class SearchUserForm(
    IOverlayService overlayService,
    IAsyncExecutor asyncExecutor,
    IUserServiceClient userServiceClient,
    bool onlyFriends = false)
    : OverlayBase
{
    public event Action<User>? OnPickUser;
    
    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private List<User> _users = [];

    public override Task<Result> LoadDataAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Ok());
    }
    
    [RelayCommand]
    private void ShowUserCard(User user)
    {
        overlayService.Show<UserCard>(user.Id);
    }
    
    [RelayCommand]
    private void PickUser(User user)
    {
        OnPickUser?.Invoke(user);
    }
    
    [RelayCommand]
    private void Close()
    {
        overlayService.Close();
    }

    [RelayCommand]
    private void SearchUser()
    {
        asyncExecutor.Execute(async cancellationToken =>
        {
            var usersResult = await userServiceClient.SearchUserAsync(Query, onlyFriends, cancellationToken);
            if (usersResult.IsFailed)
            {
                return usersResult.ToResult();
            }
            
            Users = usersResult.Value;
            return Result.Ok();
        });
    }
}