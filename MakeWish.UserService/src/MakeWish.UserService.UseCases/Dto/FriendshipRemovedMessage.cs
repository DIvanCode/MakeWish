using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.Interfaces.MessageBus;

namespace MakeWish.UserService.UseCases.Dto;

public sealed class FriendshipRemovedMessage(FriendshipRemovedMessagePayload payload) : Message
{
    public override string Type => "friendship.removed";
    public override object Payload => payload;
}

public sealed record FriendshipRemovedMessagePayload
{
    [JsonPropertyName("firstUser"), Required] public required Guid FirstUserId;
    [JsonPropertyName("secondUser"), Required] public required Guid SecondUserId;
}