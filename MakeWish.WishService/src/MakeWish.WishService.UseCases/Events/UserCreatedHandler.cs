using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using MediatR;

namespace MakeWish.WishService.UseCases.Events;

public sealed record UserCreatedNotification(Guid Id, string Name, string Surname) : INotification;

public sealed class UserCreatedHandler(IUnitOfWork unitOfWork) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification request, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is not null)
        {
            return;
        }

        user = new User(request.Id, request.Name, request.Surname);
        unitOfWork.Users.Add(user);

        var publicWishList = WishList.Create($"user-{user.Id}-public", user);
        unitOfWork.WishLists.Add(publicWishList);
        
        var privateWishList = WishList.Create($"user-{user.Id}-private", user);
        unitOfWork.WishLists.Add(privateWishList);
        
        user.PublicWishListId = publicWishList.Id;
        user.PrivateWishListId = privateWishList.Id;
        
        unitOfWork.Users.Update(user);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}