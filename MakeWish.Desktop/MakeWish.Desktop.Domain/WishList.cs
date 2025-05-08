using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed record WishList
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("owner"), Required]
    public required User Owner { get; init; }

    [JsonPropertyName("wishes")]
    public List<Wish> Wishes { get; init; } = [];
}