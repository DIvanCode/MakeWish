using FluentResults;
using MakeWish.WishService.Interfaces.Client;
using MakeWish.WishService.UseCases.Events;
using MediatR;

namespace MakeWish.WishService.UseCases.Reconciliation;

public sealed record ReconcileFriendshipsCommand : IRequest<Result>;

public sealed class ReconcileFriendshipsHandler(
    IUserServiceClient userServiceClient,
    IMediator mediator)
    : IRequestHandler<ReconcileFriendshipsCommand, Result>
{
    public async Task<Result> Handle(ReconcileFriendshipsCommand request, CancellationToken cancellationToken)
    {
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