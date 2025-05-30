using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.UseCases.Abstractions.Dto;

public sealed class UserDto
{
    [JsonPropertyName("Id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("Name"), Required]
    public required string Name { get; init; }

    [JsonPropertyName("Surname"), Required]
    public required string Surname { get; init; }
}