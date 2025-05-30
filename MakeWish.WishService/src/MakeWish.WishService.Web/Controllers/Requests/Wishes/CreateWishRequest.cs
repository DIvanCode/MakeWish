using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

namespace MakeWish.WishService.Web.Controllers.Requests.Wishes;

public sealed record CreateWishRequest
{
    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; init; }

    public CreateWishCommand ToCommand() => new(Title, Description, IsPublic);
} 