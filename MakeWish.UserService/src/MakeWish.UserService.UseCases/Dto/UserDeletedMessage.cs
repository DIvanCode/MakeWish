using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.Interfaces.MessageBus;

namespace MakeWish.UserService.UseCases.Dto;

public sealed class UserDeletedMessage(UserDeletedMessagePayload payload) : Message
{
    public override string Type => "user.deleted";
    public override object Payload => payload;
}

public sealed record UserDeletedMessagePayload
{
    [JsonPropertyName("id"), Required] public required Guid Id;
}