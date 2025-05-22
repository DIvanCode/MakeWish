using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed class Wish
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("status"), Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public required WishStatus Status { get; init; }

    [JsonPropertyName("owner"), Required]
    public required User Owner { get; init; }

    [JsonPropertyName("promiser")]
    public User? Promiser { get; init; }

    [JsonPropertyName("completer")]
    public User? Completer { get; init; }
    
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; init; }
    
    public bool HasDescription => !string.IsNullOrEmpty(Description);
    
    public bool IsPromised => Status is WishStatus.Promised;
    public bool IsCompleted => Status is WishStatus.Completed;
    public bool IsApproved => Status is WishStatus.Approved;
    public bool IsDeleted => Status is WishStatus.Deleted;
}