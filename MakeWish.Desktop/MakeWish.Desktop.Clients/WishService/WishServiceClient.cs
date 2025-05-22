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
        var response = await HttpClient.GetAsync($"/api/Wishes/{id}", cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<List<Wish>>> SearchWishAsync(Guid userId, string query, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"/api/Wishes/user/{userId}?query={query}", cancellationToken);
        return await ParseResponse<List<Wish>>(response, cancellationToken);
    }

    public async Task<Result<Wish>> CreateWishAsync(CreateWishRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/Wishes", request, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<Wish>> UpdateWishAsync(UpdateWishRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/Wishes", request, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<Wish>> DeleteWishAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/Wishes/{id}", cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<Wish>> RestoreWishAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:restore", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<List<Wish>>> GetUserWishesAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"/api/Wishes/user/{userId}", cancellationToken);
        return await ParseResponse<List<Wish>>(response, cancellationToken);
    }

    public async Task<Result<List<Wish>>> GetPromisedWishesAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync("/api/Wishes/promised", cancellationToken);
        return await ParseResponse<List<Wish>>(response, cancellationToken);
    }
    
    public async Task<Result<List<Wish>>> GetWaitingApproveWishesAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync("/api/Wishes/waiting-approve", cancellationToken);
        return await ParseResponse<List<Wish>>(response, cancellationToken);
    }

    public async Task<Result<Wish>> PromiseAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:promise", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<Wish>> PromiseCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:promise-cancel", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<Wish>> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:complete", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }
    
    public async Task<Result<Wish>> CompleteApproveAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:complete-approve", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<Wish>> CompleteRejectAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/Wishes/{id}/:complete-reject", null, cancellationToken);
        return await ParseResponse<Wish>(response, cancellationToken);
    }

    public async Task<Result<WishList>> GetWishListAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/WishLists/{id}", cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }
    
    public async Task<Result<WishList>> CreateWishListAsync(CreateWishListRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsJsonAsync("api/WishLists", request, cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }

    public async Task<Result<WishList>> UpdateWishListAsync(UpdateWishListRequest request, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PutAsJsonAsync("api/WishLists", request, cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }
    
    public async Task<Result> DeleteWishListAsync(Guid id, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.DeleteAsync($"api/WishLists/{id}", cancellationToken);
        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<WishList>> AddWishToWishListAsync(Guid id, Guid wishId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/WishLists/{id}/:add-wish/{wishId}", null, cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);        
    }
    
    public async Task<Result<WishList>> RemoveWishFromWishListAsync(Guid id, Guid wishId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/WishLists/{id}/:remove-wish/{wishId}", null, cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);        
    }

    public async Task<Result<List<WishList>>> GetUserWishListsAsync(Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/WishLists/user/{userId}", cancellationToken);
        return await ParseResponse<List<WishList>>(response, cancellationToken);
    }
    
    public async Task<Result> AllowUserAccessToWishListAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/WishLists/{id}/:allow-user-access/{userId}", null, cancellationToken);
        return await ParseResponse(response, cancellationToken);        
    }
    
    public async Task<Result> DenyUserAccessToWishListAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.PostAsync($"api/WishLists/{id}/:deny-user-access/{userId}", null, cancellationToken);
        return await ParseResponse(response, cancellationToken);        
    }
    
    public async Task<Result<List<User>>> GetUsersWithAccessToWishListAsync(Guid wishListId, CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync($"api/WishLists/{wishListId}/users-with-access", cancellationToken);
        return await ParseResponse<List<User>>(response, cancellationToken);
    }
}