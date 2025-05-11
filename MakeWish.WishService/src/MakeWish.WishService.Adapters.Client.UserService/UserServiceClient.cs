using System.Net.Http.Json;
using FluentResults;
using MakeWish.WishService.Adapters.Client.UserService.Configuration;
using MakeWish.WishService.Adapters.Client.UserService.Models;
using MakeWish.WishService.Interfaces.Client;
using MakeWish.WishService.Interfaces.Client.Responses;
using Microsoft.Extensions.Options;

namespace MakeWish.WishService.Adapters.Client.UserService;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(
        IHttpClientFactory httpClientFactory, 
        IOptions<UserServiceOptions> options)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
    }

    public async Task<Result<List<UserResponse>>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/users", cancellationToken);
        return await ParseResponse<List<UserResponse>>(response, cancellationToken);
    }
    
    public async Task<Result<List<FriendshipResponse>>> GetAllFriendshipsAsync(CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/friendships", cancellationToken);
        return await ParseResponse<List<FriendshipResponse>>(response, cancellationToken);
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
}  