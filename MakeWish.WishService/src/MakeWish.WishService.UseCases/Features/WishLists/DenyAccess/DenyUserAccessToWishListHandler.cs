using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Features.WishLists.AllowAccess;
using MakeWish.WishService.UseCases.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AllowReadCancel;

public sealed class DenyUserAccessToWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    : IRequestHandler<AllowUserAccessToWishListCommand, Result>
{
    public async Task<Result> Handle(AllowUserAccessToWishListCommand request, CancellationToken cancellationToken)
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
        
        var wishList = await unitOfWork.WishLists.GetByIdAsync(request.WishListId, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.WishListId);
        }
        
        if (!wishList.CanUserManageAccess(currentUser))
        {
            return new ForbiddenError(nameof(WishList), "cancel allow to read", nameof(WishList.Id), request.WishListId);
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
        
        unitOfWork.WishLists.DenyUserAccess(wishList, targetUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}