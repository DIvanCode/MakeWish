using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AddWish;

public sealed class AddWishToWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<AddWishToWishListCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(AddWishToWishListCommand request, CancellationToken cancellationToken)
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

        var addResult = wishList.Add(wish, by: user);
        if (addResult.IsFailed)
        {
            return addResult;
        }
        
        unitOfWork.WishLists.AddWish(wishList, wish);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new WishListDto(
            wishList.Id,
            wishList.Title,
            wishList.Owner.Id,
            wishList.Wishes.Select(w => new WishDto(
                w.Id,
                w.Title,
                Description: w.Description,
                Status: w.GetStatusFor(user),
                w.Owner.Id)).ToList());
    }
}