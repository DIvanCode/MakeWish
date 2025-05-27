using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed class WishList
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("owner"), Required]
    public required User Owner { get; set; }

    [JsonPropertyName("wishes")]
    public List<Wish> Wishes { get; set; } = [];
}