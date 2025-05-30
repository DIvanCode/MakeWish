using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Events;

public sealed record UserCreatedEvent
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }
    
    [JsonPropertyName("name"), Required]
    public required string Name { get; init; }
    
    [JsonPropertyName("surname"), Required]
    public required string Surname { get; init; }

    public UserCreatedNotification ToNotification() => new(Id, Name, Surname);
}