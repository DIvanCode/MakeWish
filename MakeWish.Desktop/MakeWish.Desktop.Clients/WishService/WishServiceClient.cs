using FluentResults;
using MakeWish.Desktop.Clients.Common.ServiceClient;
using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.WishService.Configuration;
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
    
    public async Task<Result<WishList>> GetMainWishListAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync("api/wishlists/main", cancellationToken);
        return await ParseResponse<WishList>(response, cancellationToken);
    }

    public async Task<Result<List<WishList>>> GetWishListsAsync(CancellationToken cancellationToken)
    {
        AddAuthorizationHeader();
        var response = await HttpClient.GetAsync("api/wishlists", cancellationToken);
        return await ParseResponse<List<WishList>>(response, cancellationToken);
    }
}