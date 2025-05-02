using MakeWish.UserService.Interfaces.MessageBus;
using MakeWish.UserService.Models.Events;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Events;

public sealed class FriendshipRemovedHandler(IMessagePublisher messagePublisher) : INotificationHandler<FriendshipRemovedEvent>
{
    public async Task Handle(FriendshipRemovedEvent @event, CancellationToken cancellationToken)
    {
        var message = new FriendshipRemovedMessage(
            new FriendshipRemovedMessagePayload
            {
                FirstUserId = @event.Friendship.FirstUser.Id,
                SecondUserId = @event.Friendship.SecondUser.Id
            });

        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}