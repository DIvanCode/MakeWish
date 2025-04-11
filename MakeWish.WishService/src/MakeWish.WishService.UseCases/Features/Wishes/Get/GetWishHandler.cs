using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.Get;

public sealed class GetWishHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetWishCommand, Result<WishDto>>
{
    public async Task<Result<WishDto>> Handle(GetWishCommand request, CancellationToken cancellationToken)
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
        
        var wish = await unitOfWork.Wishes.GetByIdAsync(request.WishId, cancellationToken);
        if (wish is null)
        {
            return new EntityNotFoundError(nameof(Wish), nameof(Wish.Id), request.WishId);
        }

        var existsWishListContainingWishWithUserAccess = await unitOfWork.WishLists
            .ExistsContainingWishWithUserAccessAsync(wish, user, cancellationToken);

        var hasAccess = wish.IsAccessible(to: user, existsWishListContainingWishWithUserAccess);
        if (!hasAccess)
        {
            return new ForbiddenError(nameof(Wish), "get", nameof(Wish.Id), request.WishId);
        }

        return WishDto.FromWish(wish, currUser: user);
    }
}