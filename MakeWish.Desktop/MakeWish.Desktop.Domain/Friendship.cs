using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed class Friendship
{
    [JsonPropertyName("firstUser"), Required]
    public required User FirstUser { get; set; }
    
    [JsonPropertyName("secondUser"), Required]
    public required User SecondUser { get; set; }
    
    [JsonPropertyName("isConfirmed")]
    public bool IsConfirmed { get; set; }
}