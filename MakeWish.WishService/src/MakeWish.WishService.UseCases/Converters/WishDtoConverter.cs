using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;

namespace MakeWish.WishService.UseCases.Converters;

internal static class WishDtoConverter
{
    public static WishDto Convert(Wish wish, User currUser) => new()
    {
        Id = wish.Id,
        Title = wish.Title,
        Description = wish.Description,
        Status = wish.GetStatusFor(currUser).ToString(),
        Owner = UserDtoConverter.Convert(wish.Owner)!,
        Promiser = UserDtoConverter.Convert(wish.GetPromiserFor(currUser)),
        Completer = UserDtoConverter.Convert(wish.GetCompleter()),
        IsPublic = wish.IsPublic
    };
}