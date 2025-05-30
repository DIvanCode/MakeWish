using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentResults;
using MakeWish.WishService.Adapters.Client.UserService.Configuration;
using MakeWish.WishService.Adapters.Client.UserService.Models;
using MakeWish.WishService.Interfaces.Client;
using MakeWish.WishService.Interfaces.Client.Responses;
using MakeWish.WishService.UseCases.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace MakeWish.WishService.Adapters.Client.UserService;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;

    public UserServiceClient(
        IHttpClientFactory httpClientFactory,
        IOptions<UserServiceOptions> options,
        IUserContext userContext)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userContext.Token);
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
            try
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
                return Result.Fail(problem?.Detail ?? "Request failed");
            }
            catch (Exception)
            {
                return Result.Fail(await response.Content.ReadAsStringAsync(cancellationToken));
            }
        }
        
        var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return value is not null ? Result.Ok(value) : Result.Fail("Request failed");
    }
}  