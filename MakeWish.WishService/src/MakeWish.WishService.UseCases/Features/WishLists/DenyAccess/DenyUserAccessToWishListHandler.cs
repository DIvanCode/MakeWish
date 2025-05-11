using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.DenyAccess;

public sealed class DenyUserAccessToWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<DenyUserAccessToWishListCommand, Result>
{
    public async Task<Result> Handle(DenyUserAccessToWishListCommand request, CancellationToken cancellationToken)
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
        
        var wishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(request.Id, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.Id);
        }
        
        if (!wishList.CanUserManageAccess(currentUser))
        {
            return new ForbiddenError(nameof(WishList), "deny access", nameof(WishList.Id), request.Id);
        }
        
        var targetUser = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (targetUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var hasAccess = await unitOfWork.WishLists.HasUserAccessAsync(wishList, targetUser, cancellationToken);
        if (!hasAccess)
        {
            return Result.Ok();
        }
        
        unitOfWork.WishLists.DenyUserAccess(wishList, targetUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}