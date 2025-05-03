using MakeWish.UserService.Interfaces.MessageBus;
using MakeWish.UserService.Models.Events;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Events;

public sealed class UserDeletedHandler(IMessagePublisher messagePublisher) : INotificationHandler<UserDeletedEvent>
{
    public async Task Handle(UserDeletedEvent @event, CancellationToken cancellationToken)
    {
        var message = new UserDeletedMessage(
            new UserDeletedMessagePayload
            {
                Id = @event.User.Id
            });

        await messagePublisher.PublishAsync(message, cancellationToken);
    }
}