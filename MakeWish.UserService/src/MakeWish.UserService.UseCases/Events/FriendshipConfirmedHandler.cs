using MakeWish.UserService.Interfaces.MessageBus;
using MakeWish.UserService.Models.Events;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Events;

public sealed class FriendshipConfirmedHandler(IMessagePublisher messagePublisher) : INotificationHandler<FriendshipConfirmedEvent>
{
    public async Task Handle(FriendshipConfirmedEvent @event, CancellationToken cancellationToken)
    {
        var message = new FriendshipConfirmedMessage(
            new FriendshipConfirmedMessagePayload
            {
                FirstUserId = @event.Friendship.FirstUser.Id,
                SecondUserId = @event.Friendship.SecondUser.Id
            });

        await messagePublisher.PublishAsync(message);
    }
}