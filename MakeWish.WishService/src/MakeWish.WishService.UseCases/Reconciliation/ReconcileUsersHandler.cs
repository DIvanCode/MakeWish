using FluentResults;
using MakeWish.WishService.Interfaces.Client;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.UseCases.Events;
using MediatR;

namespace MakeWish.WishService.UseCases.Reconciliation;

public sealed record ReconcileUsersCommand : IRequest<Result>;

public sealed class ReconcileUsersHandler(
    IUnitOfWork unitOfWork,
    IUserServiceClient userServiceClient,
    IMediator mediator)
    : IRequestHandler<ReconcileUsersCommand, Result>
{
    public async Task<Result> Handle(ReconcileUsersCommand request, CancellationToken cancellationToken)
    {
        var usersResult = await userServiceClient.GetAllUsersAsync(cancellationToken);
        if (usersResult.IsFailed)
        {
            return Result.Fail(usersResult.Errors.First());
        }

        var users = usersResult.Value.ToList();
        foreach (var user in users)
        {
            var localUser = await unitOfWork.Users.GetByIdAsync(user.Id, cancellationToken);
            if (localUser is null)
            {
                var notification = new UserCreatedNotification(user.Id, user.Name, user.Surname);
                await mediator.Publish(notification, cancellationToken);
                continue;
            }

            if (localUser.Name == user.Name && localUser.Surname == user.Surname)
            {
                continue;
            }
            
            localUser.Name = user.Name;
            localUser.Surname = user.Surname;
            unitOfWork.Users.Update(localUser);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        var friendshipsResult = await userServiceClient.GetAllFriendshipsAsync(cancellationToken);
        if (friendshipsResult.IsFailed)
        {
            return Result.Fail(friendshipsResult.Errors.First());
        }

        var friendships = friendshipsResult.Value.ToList();
        foreach (var friendship in friendships)
        {
            var notification = new FriendshipConfirmedNotification(friendship.FirstUser.Id, friendship.SecondUser.Id);
            await mediator.Publish(notification, cancellationToken);
        }

        return Result.Ok();
    }
}