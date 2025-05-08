using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Clients.WishService.Requests.Wishes;

public sealed record CreateWishRequest
{
    [JsonPropertyName("title"), Required] 
    public required string Title { get; init; }
    
    [JsonPropertyName("description"), Required] 
    public required string Description { get; init; }
}