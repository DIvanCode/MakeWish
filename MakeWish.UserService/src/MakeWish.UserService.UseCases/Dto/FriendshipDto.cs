using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.UserService.Models;
using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.UseCases.Dto;

public sealed record FriendshipDto
{
    [JsonPropertyName("firstUser"), Required]
    public required UserDto FirstUser { get; init; }
    
    [JsonPropertyName("secondUser"), Required]
    public required UserDto SecondUser { get; init; }
    
    [JsonPropertyName("isConfirmed"), Required]
    public required bool IsConfirmed { get; init; }

    private FriendshipDto()
    {
    }

    public static FriendshipDto FromFriendship(Friendship friendship) => new()
    {
        FirstUser = UserDto.FromUser(friendship.FirstUser),
        SecondUser = UserDto.FromUser(friendship.SecondUser),
        IsConfirmed = friendship.IsConfirmed,
    };
}