using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MakeWish.WishService.UseCases.Events;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Events;

public sealed record FriendshipRemovedEvent
{
    [JsonPropertyName("firstUser"), Required]
    public required Guid FirstUserId  { get; init; }
    
    [JsonPropertyName("secondUser"), Required]
    public required Guid SecondUserId  { get; init; }
    
    public FriendshipRemovedNotification ToNotification() => new(FirstUserId, SecondUserId);
}