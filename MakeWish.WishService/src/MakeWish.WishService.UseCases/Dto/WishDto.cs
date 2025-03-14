using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record WishDto(
    [property: JsonPropertyName("id"), Required] Guid Id,
    [property: JsonPropertyName("title"), Required] string Title,
    [property: JsonPropertyName("description"), Required] string? Description,
    [property: JsonPropertyName("status"), Required] string Status,
    [property: JsonPropertyName("ownerId"), Required] Guid OwnerId,
    [property: JsonPropertyName("promiserId"), Required] Guid? PromiserId = null,
    [property: JsonPropertyName("completerId"), Required] Guid? CompleterId = null);