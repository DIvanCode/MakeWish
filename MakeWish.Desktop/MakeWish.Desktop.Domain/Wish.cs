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

    public string DisplayStatus => Status switch
    {
        WishStatus.Created => "Создано",
        WishStatus.Promised => "Обещано",
        WishStatus.Completed => "Выполнено",
        WishStatus.Approved => "Подтверждено",
        WishStatus.Deleted => "Удалено",
        _ => "Unknown"
    };

    public int StatusOrder => Status switch
    {
        WishStatus.Created => 0,
        WishStatus.Promised => 1,
        WishStatus.Completed => 2,
        WishStatus.Approved => 3,
        WishStatus.Deleted => 4,
        _ => 55
    };
    
    public bool IsPromised => Status is WishStatus.Promised;
    public bool IsCompleted => Status is WishStatus.Completed;
    public bool IsApproved => Status is WishStatus.Approved;
    public bool IsDeleted => Status is WishStatus.Deleted;
}