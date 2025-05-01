using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.UseCases.Features.Friendships.Confirm;

namespace MakeWish.UserService.Web.Controllers.Requests.Friendships;

public sealed record ConfirmFriendshipRequest
{
    [JsonPropertyName("firstUser"), Required] public required Guid FirstUserId { get; init; }
    [JsonPropertyName("secondUser"), Required] public required Guid SecondUserId { get; init; }

    public ConfirmFriendshipCommand ToCommand() => new(FirstUserId, SecondUserId);
}