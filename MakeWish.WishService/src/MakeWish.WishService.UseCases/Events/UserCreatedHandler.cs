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
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}