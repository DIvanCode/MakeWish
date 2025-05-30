using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Events;

public sealed record UserDeletedEvent
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    public UserDeletedNotification ToNotification() => new(Id);
}