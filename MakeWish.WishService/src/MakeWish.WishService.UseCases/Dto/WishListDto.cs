using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.UseCases.Dto;

public sealed record WishListDto
{
    [JsonPropertyName("id"), Required]
    public required Guid Id { get; init; }

    [JsonPropertyName("title"), Required]
    public required string Title { get; init; }

    [JsonPropertyName("owner"), Required]
    public required Guid OwnerId { get; init; }

    [JsonPropertyName("wishes"), Required]
    public required IReadOnlyList<WishDto> Wishes { get; init; }

    private WishListDto()
    {
    }

    public static WishListDto FromWishList(WishList wishList, User currUser, bool excludeWishes = false) => new()
    {
        Id = wishList.Id,
        Title = wishList.Title,
        OwnerId = wishList.Owner.Id,
        Wishes = excludeWishes ? [] : wishList.Wishes.Select(wish => WishDto.FromWish(wish, currUser)).ToList()
    };
}