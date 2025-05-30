using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;

namespace MakeWish.WishService.UseCases.Converters;

internal static class WishListDtoConverter
{
    public static WishListDto Convert(WishList wishList, User currUser, bool excludeWishes = false) => new()
    {
        Id = wishList.Id,
        Title = wishList.Title,
        Owner = UserDtoConverter.Convert(wishList.Owner)!,
        Wishes = excludeWishes ? [] : wishList.Wishes.Select(wish => WishDtoConverter.Convert(wish, currUser)).ToList()
    };
}