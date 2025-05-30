using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;

namespace MakeWish.WishService.UseCases.Converters;

internal static class UserDtoConverter
{
    public static UserDto? Convert(User? user) => user is null ? null : new UserDto
    {
        Id = user.Id,
        Name = user.Name,
        Surname = user.Surname
    };
}