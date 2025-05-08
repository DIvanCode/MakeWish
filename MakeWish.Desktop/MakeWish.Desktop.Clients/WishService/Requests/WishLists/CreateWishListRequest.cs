using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Clients.WishService.Requests.WishLists;

public sealed record CreateWishListRequest
{
    [JsonPropertyName("title"), Required] 
    public required string Title { get; init; }
}