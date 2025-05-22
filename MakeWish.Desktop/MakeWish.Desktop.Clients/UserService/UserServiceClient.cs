using System.Net.Http.Json;
using FluentResults;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Clients.Common.ServiceClient;
using MakeWish.Desktop.Clients.Common.UserContext;
using Microsoft.Extensions.Options;

namespace MakeWish.Desktop.Clients.UserService;

public class UserServiceClient : ServiceClient, IUserServiceClient
{
    public UserServiceClient(
        IHttpClientFactory httpClientFactory,
        IUserContext userContext, 
        IOptions<UserServiceOptions> options): base(httpClientFactory, userContext, options.Value.BaseUrl)
    {
    }

    public async Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/Users/{id}", cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }

    public async Task<Result<List<User>>> SearchUserAsync(string query, bool onlyFriends, CancellationToken cancellationToken)
    {
        var response = await HttpClient.GetAsync($"api/Users?query={query}&onlyFriends={onlyFriends}", cancellationToken);
        return await ParseResponse<List<User>>(response, cancellationToken);   
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/Users/{id}", cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<string>> AuthenticateUserAsync(AuthenticateRequest request, CancellationToken cancellationToken)
    {
        var response = await HttpClient.PostAsJsonAsync("api/Users/:authenticate", request, cancellationToken);
        return await ParseStringResponse(response, cancellationToken);
    }

    public async Task<Result<User>> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await HttpClient.PostAsJsonAsync("api/Users/:register", request, cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }

    public async Task<Result<User>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync("api/Users/:current", cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }

    public async Task<Result<Friendship>> CreateFriendshipAsync(CreateFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Friendships", request, cancellationToken);
        return await ParseResponse<Friendship>(response, cancellationToken);
    }

    public async Task<Result<Friendship>> ConfirmFriendshipAsync(ConfirmFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Friendships/:confirm", request, cancellationToken);
        return await ParseResponse<Friendship>(response, cancellationToken);
    }

    public async Task<Result> RemoveFriendshipAsync(RemoveFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        
        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "api/Friendships");
        httpRequest.Content = JsonContent.Create(request);

        var response = await HttpClient.SendAsync(httpRequest, cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetConfirmedFriendshipsAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/Friendships/confirmed/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetPendingFriendshipsFromUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/Friendships/pending-from/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetPendingFriendshipsToUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/Friendships/pending-to/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }
} 