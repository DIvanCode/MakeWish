using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Features.WishLists.Create;

namespace MakeWish.WishService.Web.Controllers.Requests.WishLists;

public sealed record CreateWishListRequest
{
    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    public CreateWishListCommand ToCommand() => new(Title);
} 