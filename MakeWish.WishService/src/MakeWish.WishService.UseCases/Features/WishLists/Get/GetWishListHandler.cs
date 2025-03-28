using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Get;

public sealed class GetWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetWishListCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(GetWishListCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }
        
        var wishList = await unitOfWork.WishLists.GetByIdAsync(request.WishListId, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.WishListId);
        }

        var hasAccess = wishList.Owner.Id == user.Id ||
                        await unitOfWork.WishLists.HasUserAccessAsync(wishList, user, cancellationToken);
        if (!hasAccess)
        {
            return new ForbiddenError(nameof(WishList), "get", nameof(WishList.Id), request.WishListId);
        }

        return new WishListDto(
            wishList.Id,
            wishList.Title,
            wishList.Owner.Id,
            wishList.Wishes.Select(w => new WishDto(
                w.Id,
                w.Title,
                Description: null,
                Status: w.GetStatusFor(user),
                w.Owner.Id)).ToList());
    }
}