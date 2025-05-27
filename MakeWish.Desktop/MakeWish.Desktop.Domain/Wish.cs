using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Domain;

public sealed class Wish
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("status"), Required, JsonConverter(typeof(JsonStringEnumConverter))]
    public required WishStatus Status { get; set; }

    [JsonPropertyName("owner"), Required]
    public required User Owner { get; set; }

    [JsonPropertyName("promiser")]
    public User? Promiser { get; set; }

    [JsonPropertyName("completer")]
    public User? Completer { get; set; }
    
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; }
    
    public bool HasDescription => !string.IsNullOrEmpty(Description);
    
    public bool IsPromised => Status is WishStatus.Promised;
    public bool IsCompleted => Status is WishStatus.Completed;
    public bool IsApproved => Status is WishStatus.Approved;
    public bool IsDeleted => Status is WishStatus.Deleted;
}