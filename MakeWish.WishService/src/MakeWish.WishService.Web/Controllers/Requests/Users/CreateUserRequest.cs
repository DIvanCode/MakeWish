using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Features.Users.Create;

namespace MakeWish.WishService.Web.Controllers.Requests.Users;

public sealed record CreateUserRequest
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("name"), Required]
    public required string Name { get; init; }
    
    [JsonPropertyName("surname"), Required]
    public required string Surname { get; init; }

    public CreateUserCommand ToCommand() => new(Id, Name, Surname);
} 