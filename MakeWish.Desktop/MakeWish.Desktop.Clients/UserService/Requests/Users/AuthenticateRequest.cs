using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Clients.UserService.Requests.Users;

public sealed record AuthenticateRequest
{
    [JsonPropertyName("email"), Required] 
    public required string Email { get; init; }
    
    [JsonPropertyName("password"), Required] 
    public required string Password { get; init; }
}