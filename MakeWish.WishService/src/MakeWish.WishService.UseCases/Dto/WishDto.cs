using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record WishDto
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    [JsonPropertyName("description"), Required]
    public required string Description { get; init; }

    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    [JsonPropertyName("status"), Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public required WishStatus Status { get; init; }

    [JsonPropertyName("owner"), Required]
    public required UserDto Owner { get; init; }

    [JsonPropertyName("promiser")]
    public UserDto? Promiser { get; private init; }

    [JsonPropertyName("completer")]
    public UserDto? Completer { get; private init; }

    private WishDto()
    {
    }

    public static WishDto FromWish(Wish wish, User currUser) => new()
    {
        Id = wish.Id,
        Title = wish.Title,
        Description = wish.Description,
        ImageUrl = wish.ImageUrl,
        Status = wish.GetStatusFor(currUser),
        Owner = UserDto.FromUser(wish.Owner)!,
        Promiser = UserDto.FromUser(wish.GetPromiserFor(currUser)),
        Completer = UserDto.FromUser(wish.GetCompleter())
    };
}