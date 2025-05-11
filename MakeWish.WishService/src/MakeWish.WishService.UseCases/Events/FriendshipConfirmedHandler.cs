using MakeWish.WishService.Interfaces.DataAccess;
using MediatR;

namespace MakeWish.WishService.UseCases.Events;

public sealed record FriendshipConfirmedNotification(Guid FirstUserId, Guid SecondUserId) : INotification;

public sealed class FriendshipConfirmedHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<FriendshipConfirmedNotification>
{
    public async Task Handle(FriendshipConfirmedNotification request, CancellationToken cancellationToken)
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
        
        unitOfWork.WishLists.AllowUserAccess(firstUserPublicWishList!, secondUser);
        unitOfWork.WishLists.AllowUserAccess(secondUserPublicWishList!, firstUser);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}