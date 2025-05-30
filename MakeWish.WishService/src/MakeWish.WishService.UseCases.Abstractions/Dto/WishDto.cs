using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.UseCases.Abstractions.Dto;

public sealed class WishDto
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    [JsonPropertyName("description"), Required]
    public required string Description { get; init; }

    [JsonPropertyName("status"), Required]
    public required string Status { get; init; }

    [JsonPropertyName("owner"), Required]
    public required UserDto Owner { get; init; }

    [JsonPropertyName("promiser")]
    public UserDto? Promiser { get; init; }

    [JsonPropertyName("completer")]
    public UserDto? Completer { get; init; }

    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; init; }
}