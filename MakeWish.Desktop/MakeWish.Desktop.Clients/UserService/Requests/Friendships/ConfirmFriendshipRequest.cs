using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MakeWish.Desktop.Clients.UserService.Requests.Friendships;

public sealed record ConfirmFriendshipRequest
{
    [JsonPropertyName("firstUser"), Required]
    public required Guid FirstUser { get; init; }
    
    [JsonPropertyName("secondUser"), Required]
    public required Guid SecondUser { get; init; }
}