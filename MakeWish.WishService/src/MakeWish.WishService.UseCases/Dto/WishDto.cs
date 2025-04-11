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
    private string? ImageUrl { get; init; }

    [JsonPropertyName("status"), Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public required WishStatus Status { get; init; }

    [JsonPropertyName("ownerId"), Required]
    public required Guid OwnerId { get; init; }

    [JsonPropertyName("promiserId")]
    public Guid? PromiserId { get; private init; }

    [JsonPropertyName("completerId")]
    public Guid? CompleterId { get; private init; }

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
        OwnerId = wish.Owner.Id,
        PromiserId = wish.GetPromiserFor(currUser)?.Id,
        CompleterId = wish.GetCompleter()?.Id
    };
}