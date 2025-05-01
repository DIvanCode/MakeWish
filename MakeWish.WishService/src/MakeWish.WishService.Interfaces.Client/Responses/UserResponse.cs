using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.Interfaces.Client.Responses;

public sealed record UserResponse
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("name"), Required]
    public required string Name { get; init; }
    
    [JsonPropertyName("surname"), Required]
    public required string Surname  { get; init; }
}