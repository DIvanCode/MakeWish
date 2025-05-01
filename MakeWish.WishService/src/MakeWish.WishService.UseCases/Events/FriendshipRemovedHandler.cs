using MakeWish.WishService.Interfaces.DataAccess;
using MediatR;

namespace MakeWish.WishService.UseCases.Events;

public sealed record FriendshipRemovedNotification(Guid FirstUserId, Guid SecondUserId) : INotification;

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

        var firstUserMainWishList = await unitOfWork.WishLists.GetMainForUserAsync(firstUser, cancellationToken);
        var secondUserMainWishList = await unitOfWork.WishLists.GetMainForUserAsync(secondUser, cancellationToken);
        
        unitOfWork.WishLists.DenyUserAccess(firstUserMainWishList, secondUser);
        unitOfWork.WishLists.DenyUserAccess(secondUserMainWishList, firstUser);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}