using System.Net.Http.Json;
using FluentResults;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.UserService.Models;
using MakeWish.Desktop.Clients.UserService.Requests.Friendships;
using MakeWish.Desktop.Clients.UserService.Requests.Users;
using MakeWish.Desktop.Domain;
using MakeWish.Desktop.Clients.UserContext;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace MakeWish.Desktop.Clients.UserService;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly IUserContext _userContext;

    public UserServiceClient(
        IHttpClientFactory httpClientFactory, 
        IOptions<UserServiceOptions> options,
        IUserContext userContext)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _userContext = userContext;
    }

    public async Task<Result<User>> GetUserAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync($"api/users/{id}", cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }

    public async Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"api/users/{id}", cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<string>> AuthenticateUserAsync(AuthenticateRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/:authenticate", request, cancellationToken);
        return await ParseStringResponse(response, cancellationToken);
    }

    public async Task<Result<User>> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users/:register", request, cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }

    public async Task<Result<Friendship>> CreateFriendshipAsync(CreateFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("api/friendships", request, cancellationToken);
        return await ParseResponse<Friendship>(response, cancellationToken);
    }

    public async Task<Result<Friendship>> ConfirmFriendshipAsync(ConfirmFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("api/friendships/:confirm", request, cancellationToken);
        return await ParseResponse<Friendship>(response, cancellationToken);
    }

    public async Task<Result> RemoveFriendshipAsync(RemoveFriendshipRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        
        using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "api/friendships");
        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetConfirmedFriendshipsAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync($"api/friendships/confirmed/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetPendingFriendshipsFromUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync($"api/friendships/pending/from/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }

    public async Task<Result<List<Friendship>>> GetPendingFriendshipsToUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync($"api/friendships/pending/to/{userId}", cancellationToken);
        return await ParseResponse<List<Friendship>>(response, cancellationToken);
    }

    public async Task<Result<User>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.GetAsync("api/users/:current", cancellationToken);
        return await ParseResponse<User>(response, cancellationToken);
    }
    
    
    private void AddAuthorizationHeader()
    {
        var token = _userContext.Token;
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private static async Task<Result<T>> ParseResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
            return Result.Fail(problem?.Detail ?? "Request failed");
        }
        
        var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return value is not null ? Result.Ok(value) : Result.Fail("Request failed");
    }
    
    private static async Task<Result<string>> ParseStringResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(await response.Content.ReadAsStringAsync(cancellationToken));
        }
        
        var value = await response.Content.ReadAsStringAsync(cancellationToken);
        return Result.Ok(value);
    }
    
    private static async Task<Result> ParseResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            try
            {
                var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(body);
                if (problemDetails != null)
                {

                    return Result.Fail(problemDetails.Detail);
                }
            }
            catch (Exception ex)
            {
                // ignored
            }

            return Result.Fail(body);
        }

        return Result.Ok();
    }
} 