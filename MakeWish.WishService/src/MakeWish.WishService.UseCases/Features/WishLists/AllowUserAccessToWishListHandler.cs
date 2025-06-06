﻿using FluentResults;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MakeWish.WishService.UseCases.Abstractions.Features.WishLists;
using MakeWish.WishService.UseCases.Abstractions.Services;
using MakeWish.WishService.Utils.Errors;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists;

public sealed class AllowUserAccessToWishListHandler(IUserContext userContext, IUnitOfWork unitOfWork)
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
        
        var wishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(request.Id, cancellationToken);
        if (wishList is null)
        {
            return new EntityNotFoundError(nameof(WishList), nameof(WishList.Id), request.Id);
        }

        var canUserManageAccess = wishList.CanUserManageAccess(currentUser);
        if (!canUserManageAccess)
        {
            return new ForbiddenError(nameof(WishList), "allow access", nameof(WishList.Id), request.Id);
        }
        
        var targetUser = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (targetUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var hasAccess = await unitOfWork.WishLists.HasUserAccessAsync(wishList, targetUser, cancellationToken);
        if (hasAccess)
        {
            return Result.Ok();
        }
        
        unitOfWork.WishLists.AllowUserAccess(wishList, targetUser);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}