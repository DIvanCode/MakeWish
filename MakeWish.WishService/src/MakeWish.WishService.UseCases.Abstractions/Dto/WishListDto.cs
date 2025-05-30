using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.UseCases.Abstractions.Dto;

public sealed record WishListDto
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    [JsonPropertyName("owner"), Required]
    public required UserDto Owner { get; init; }

    [JsonPropertyName("wishes"), Required]
    public required IReadOnlyList<WishDto> Wishes { get; init; }
}