using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.UserService.UseCases.Dto;

public sealed record UserDto(
    [property: JsonPropertyName("id"), Required] Guid Id,
    [property: JsonPropertyName("email"), Required] string Email,
    [property: JsonPropertyName("name"), Required] string Name,
    [property: JsonPropertyName("surname"), Required] string Surname);