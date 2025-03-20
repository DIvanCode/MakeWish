using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.UseCases.Features.Users.Authenticate;

namespace MakeWish.UserService.Web.Controllers.Requests.Users;

public class AuthenticateRequest
{
    [JsonPropertyName("email"), Required] public required string Email { get; init; }
    [JsonPropertyName("password"), Required] public required string Password { get; init; }

    public AuthenticateCommand ToCommand() => new(Email, Password);
}