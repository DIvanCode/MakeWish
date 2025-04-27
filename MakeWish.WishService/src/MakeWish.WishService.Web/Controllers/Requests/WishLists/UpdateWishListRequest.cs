using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Features.WishLists.Update;

namespace MakeWish.WishService.Web.Controllers.Requests.WishLists;

public sealed record UpdateWishListRequest
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    public UpdateWishListCommand ToCommand() => new(Id, Title);
} 