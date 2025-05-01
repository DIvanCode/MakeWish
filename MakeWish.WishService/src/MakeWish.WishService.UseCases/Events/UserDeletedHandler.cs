using MakeWish.WishService.Interfaces.DataAccess;
using MediatR;

namespace MakeWish.WishService.UseCases.Events;

public sealed record UserDeletedNotification(Guid Id) : INotification;

public sealed class UserDeletedHandler(IUnitOfWork unitOfWork) : INotificationHandler<UserDeletedNotification>
{
    public async Task Handle(UserDeletedNotification request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
        {
            return;
        }

        var wishes = await unitOfWork.Wishes.GetWithOwnerAsync(user, cancellationToken);
        foreach (var wish in wishes)
        {
            unitOfWork.Wishes.Remove(wish);
        }
        
        var wishLists = await unitOfWork.WishLists.GetWithOwnerAsync(user, cancellationToken);
        foreach (var wishList in wishLists)
        {
            unitOfWork.WishLists.Remove(wishList);
        }
        
        unitOfWork.Users.Remove(user);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}