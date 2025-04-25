using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.UseCases.Features.Friendships.ConfirmFriendship;
using MakeWish.UserService.UseCases.Features.Friendships.RemoveFriendship;

namespace MakeWish.UserService.Web.Controllers.Requests.Friendships;

public sealed record ConfirmFriendshipRequest
{
    [JsonPropertyName("firstUser"), Required] public required Guid FirstUserId { get; init; }
    [JsonPropertyName("secondUser"), Required] public required Guid SecondUserId { get; init; }

    public ConfirmFriendshipCommand ToCommand() => new(FirstUserId, SecondUserId);
}