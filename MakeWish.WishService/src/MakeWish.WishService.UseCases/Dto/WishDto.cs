using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record WishDto(
    [property: JsonPropertyName("id"), Required] Guid Id,
    [property: JsonPropertyName("title"), Required] string Title,
    [property: JsonPropertyName("description"), Required] string Description,
    [property: JsonPropertyName("status"), Required, JsonConverter(typeof(JsonStringEnumConverter))] WishStatus Status,
    [property: JsonPropertyName("ownerId"), Required] Guid OwnerId,
    [property: JsonPropertyName("promiserId"), Required] Guid? PromiserId = null,
    [property: JsonPropertyName("completerId"), Required] Guid? CompleterId = null);