using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Features.Wishes.Update;

namespace MakeWish.WishService.Web.Controllers.Requests.Wishes;

public sealed record UpdateWishRequest
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }
    
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; init; }

    public UpdateWishCommand ToCommand() => new(Id, Title, Description ?? "", ImageUrl);
} 