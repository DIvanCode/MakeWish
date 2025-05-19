using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentResults;
using MakeWish.Desktop.Abstract;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Services;

namespace MakeWish.Desktop.Forms.Users;

public sealed partial class SearchUserForm : Form
{
    public event Action<Guid>? OnPickUser;
    
    private readonly IUserServiceClient _userServiceClient;
    
    [ObservableProperty]
    private string _query = string.Empty;

    [ObservableProperty]
    private List<User> _users = [];

    public SearchUserForm(
        INavigationService navigationService,
        IRequestExecutor requestExecutor,
        IUserServiceClient userServiceClient)
        : base(navigationService, requestExecutor)
    {
        _userServiceClient = userServiceClient;
    }
    
    [RelayCommand]
    private void ShowUserCard(Guid userId)
    {
        NavigationService.ShowOverlay<UserCard>(userId);
    }
    
    [RelayCommand]
    private void PickUser(Guid userId)
    {
        OnPickUser?.Invoke(userId);
    }
    
    [RelayCommand]
    private void Close()
    {
        NavigationService.CloseLastOverlay();
    }

    [RelayCommand]
    private void SearchUser()
    {
        RequestExecutor.Execute(async () => await SearchUserAsync(Query));
    }

    private async Task<Result> SearchUserAsync(string query)
    {
        var usersResult = await _userServiceClient.SearchUserAsync(query, CancellationToken.None);
        if (usersResult.IsFailed)
        {
            return usersResult.ToResult();
        }
            
        Users = usersResult.Value;
        return Result.Ok();
    }
}