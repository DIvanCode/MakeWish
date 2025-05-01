using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.Interfaces.MessageBus;

namespace MakeWish.UserService.UseCases.Dto;

public sealed class UserCreatedMessage(UserCreatedMessagePayload payload) : Message
{
    public override string Type => "user.created";
    public override object Payload => payload;
}

public sealed record UserCreatedMessagePayload
{
    [JsonPropertyName("id"), Required] public required Guid Id;
    [JsonPropertyName("name"), Required] public required string Name;
    [JsonPropertyName("surname"), Required] public required string Surname;
}