using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record UserDto
{
    [JsonPropertyName("Id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("Name"), Required]
    public required string Name { get; init; }

    [JsonPropertyName("Surname"), Required]
    public required string Surname { get; init; }

    public static UserDto? FromUser(User? user) => user is null ? null : new UserDto
    {
        Id = user.Id,
        Name = user.Name,
        Surname = user.Surname
    };
}