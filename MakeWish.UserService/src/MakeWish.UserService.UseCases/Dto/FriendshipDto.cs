using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.UserService.UseCases.Dto;

public record FriendshipDto(
    [property: JsonPropertyName("firstUser"), Required] Guid FirstUserId,
    [property: JsonPropertyName("secondUser"), Required] Guid SecondUserId,
    [property: JsonPropertyName("isConfirmed"), Required] bool IsConfirmed);