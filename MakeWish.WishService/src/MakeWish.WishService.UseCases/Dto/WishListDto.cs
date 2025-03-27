using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record WishListDto(
    [property: JsonPropertyName("id"), Required] Guid Id,
    [property: JsonPropertyName("title"), Required] string Title,
    [property: JsonPropertyName("owner"), Required] Guid OwnerId,
    [property: JsonPropertyName("wishes"), Required] IReadOnlyList<WishDto> Wishes);