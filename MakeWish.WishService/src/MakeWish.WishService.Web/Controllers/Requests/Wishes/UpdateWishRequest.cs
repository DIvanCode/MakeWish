using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Abstractions.Features.Wishes;

namespace MakeWish.WishService.Web.Controllers.Requests.Wishes;

public sealed record UpdateWishRequest
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; init; }

    public UpdateWishCommand ToCommand() => new(Id, Title, Description ?? "", IsPublic);
} 