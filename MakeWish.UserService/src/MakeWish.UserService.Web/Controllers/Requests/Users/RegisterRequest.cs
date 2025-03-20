using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.UseCases.Features.Users.Register;

namespace MakeWish.UserService.Web.Controllers.Requests.Users;

public sealed record RegisterRequest
{
    [JsonPropertyName("email"), Required] public required string Email { get; init; }
    [JsonPropertyName("password"), Required] public required string Password { get; init; }
    [JsonPropertyName("name"), Required] public required string Name { get; init; }
    [JsonPropertyName("surname"), Required] public required string Surname { get; init; }

    public RegisterCommand ToCommand() => new(Email, Password, Name, Surname);
}