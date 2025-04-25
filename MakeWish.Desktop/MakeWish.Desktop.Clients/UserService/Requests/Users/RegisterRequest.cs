using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Clients.UserService.Requests.Users;

public sealed record RegisterRequest
{
    [JsonPropertyName("email"), Required] 
    public required string Email { get; init; }
    
    [JsonPropertyName("password"), Required] 
    public required string Password { get; init; }
    
    [JsonPropertyName("name"), Required] 
    public required string Name { get; init; }
    
    [JsonPropertyName("surname"), Required] 
    public required string Surname { get; init; }
}