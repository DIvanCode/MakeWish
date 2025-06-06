﻿using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UseCases.Abstractions.Events;
using MediatR;

namespace MakeWish.WishService.UseCases.Events;

public sealed class FriendshipRemovedHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<FriendshipRemovedNotification>
{
    public async Task Handle(FriendshipRemovedNotification request, CancellationToken cancellationToken)
    {
        var firstUser = await unitOfWork.Users.GetByIdAsync(request.FirstUserId, cancellationToken);
        if (firstUser is null)
        {
            return;
        }
        
        var secondUser = await unitOfWork.Users.GetByIdAsync(request.SecondUserId, cancellationToken);
        if (secondUser is null)
        {
            return;
        }

        var firstUserPublicWishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(
            firstUser.PublicWishListId,
            cancellationToken);
        var secondUserPublicWishList = await unitOfWork.WishLists.GetByIdWithoutWishesAsync(
            secondUser.PublicWishListId,
            cancellationToken);
        
        unitOfWork.WishLists.DenyUserAccess(firstUserPublicWishList!, secondUser);
        unitOfWork.WishLists.DenyUserAccess(secondUserPublicWishList!, firstUser);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}