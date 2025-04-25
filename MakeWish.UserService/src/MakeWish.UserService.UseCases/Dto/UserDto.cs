using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.Models;

namespace MakeWish.UserService.UseCases.Dto;

public sealed record UserDto
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("email"), Required]
    public required string Email { get; init; }
    
    [JsonPropertyName("name"), Required]
    public required string Name { get; init; }
    
    [JsonPropertyName("surname"), Required]
    public required string Surname  { get; init; }

    private UserDto()
    {
    }

    public static UserDto FromUser(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Surname = user.Surname,
    };
}