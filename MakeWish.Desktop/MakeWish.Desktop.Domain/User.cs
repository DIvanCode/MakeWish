using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed class User
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("surname")]
    public string Surname { get; set; } = string.Empty;
    
    public string DisplayName => $"{Name} {Surname}";
}