using System.Net.Http.Json;
using FluentResults;
using MakeWish.Desktop.Clients.Common.ServiceClient;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService.Configuration;
using MakeWish.Desktop.Clients.WishService.Requests.Wishes;
using MakeWish.Desktop.Clients.WishService.Requests.WishLists;
using MakeWish.Desktop.Domain;
using Microsoft.Extensions.Options;

namespace MakeWish.Desktop.Clients.WishService;

public sealed class WishServiceClient : ServiceClient, IWishServiceClient
{
    public WishServiceClient(
        IHttpClientFactory httpClientFactory,
        IUserContext userContext,
        IOptions<WishServiceOptions> options): base(httpClientFactory, userContext, options.Value.BaseUrl)
    {
    }

    public async Task<Result<Wish>> GetWishAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"/api/wishes/{id}", cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<Wish>> CreateWishAsync(CreateWishRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/wishes", request, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<List<Wish>>> GetUserWishesAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"/api/wishes/user/{userId}", cancellationToken);
        return await ParseResponse<List<Wish>>(response, cancellationToken);
    }

    public async Task<Result<Wish>> CompleteApproveAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/wishes/{id}/:complete-approve", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<Wish>> CompleteRejectAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/wishes/{id}/:complete-reject", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<WishList>> GetWishListAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/wishlists/{id}", cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }
    
    public async Task<Result<WishList>> CreateWishListAsync(CreateWishListRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/wishlists", request, cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }

    public async Task<Result<List<WishList>>> GetUserWishListsAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/wishlists/user/{userId}", cancellationToken);
        return await ParseResponse<List<WishList>>(response, cancellationToken);
    }
}