using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.RemoveWish;

public sealed class RemoveWishFromWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveWishFromWishListCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(RemoveWishFromWishListCommand request, CancellationToken cancellationToken)
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
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.WishId, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.WishId);
        }

        var removeResult = wishList.Remove(wish, by: user);
        if (removeResult.IsFailed)
        {
            return removeResult;
        }
        
        unitOfWork.WishLists.RemoveWish(wishList, wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return WishListDto.FromWishList(wishList, currUser: user);
    }
}