using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.UserService.UseCases.Dto;

public record FriendshipDto(
    [property: JsonPropertyName("firstUser"), Required] int FirstUserId,
    [property: JsonPropertyName("secondUser"), Required] int SecondUserId,
    [property: JsonPropertyName("isConfirmed"), Required] bool IsConfirmed);