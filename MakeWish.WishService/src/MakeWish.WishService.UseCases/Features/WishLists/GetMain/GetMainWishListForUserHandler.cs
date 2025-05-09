using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetMain;

public sealed class GetMainWishListForUserHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<GetMainWishListForUserCommand, Result<WishListDto>>
{
    public async Task<Result<WishListDto>> Handle(
        GetMainWishListForUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var currentUser = await unitOfWork.Users.GetByIdAsync(userContext.UserId, cancellationToken);
        if (currentUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), userContext.UserId);
        }
        
        var targetUser = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (targetUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var wishList = await unitOfWork.WishLists.GetMainForUserAsync(targetUser, cancellationToken);
        
        var hasAccess = wishList.Owner.Id == currentUser.Id ||
                        await unitOfWork.WishLists.HasUserAccessAsync(wishList, currentUser, cancellationToken);
        if (!hasAccess)
        {
            return new ForbiddenError(nameof(WishList), "get main", nameof(WishList.Owner.Id), request.UserId);
        }

        return WishListDto.FromWishList(wishList, currUser: currentUser);
    }
}