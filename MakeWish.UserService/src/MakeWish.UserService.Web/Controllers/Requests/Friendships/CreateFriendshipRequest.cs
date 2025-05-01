using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.UseCases.Features.Friendships.Create;

namespace MakeWish.UserService.Web.Controllers.Requests.Friendships;

public sealed record CreateFriendshipRequest
{
    [JsonPropertyName("firstUser"), Required] public required Guid FirstUserId { get; init; }
    [JsonPropertyName("secondUser"), Required] public required Guid SecondUserId { get; init; }

    public CreateFriendshipCommand ToCommand() => new(FirstUserId, SecondUserId);
}