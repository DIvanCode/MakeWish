using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.Interfaces.Client.Responses;

public sealed record FriendshipResponse
{
    [JsonPropertyName("firstUser"), Required]
    public required UserResponse FirstUser { get; init; }
    
    [JsonPropertyName("secondUser"), Required]
    public required UserResponse SecondUser { get; init; }
}