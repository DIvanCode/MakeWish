using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MakeWish.WishService.UseCases.Abstractions.Features.WishLists;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.UseCases.Converters;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists;

public sealed class GetUsersWithAccessToWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetUsersWithAccessToWishListCommand, Result<List<UserDto>>>
{
    public async Task<Result<List<UserDto>>> Handle(
        GetUsersWithAccessToWishListCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var wishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(request.Id, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.Id);
        }

        if (userContext.UserId != wishList.Owner.Id)
        {
            return new ForbiddenError("Cannot get users with access to this wish list.");
        }

        var users = await unitOfWork.Users.GetUsersWithAccessToWishListAsync(wishList, cancellationToken);
        return users.Select(UserDtoConverter.Convert).ToList()!;
    }
}