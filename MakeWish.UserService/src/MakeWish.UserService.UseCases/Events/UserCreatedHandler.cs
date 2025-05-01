using MakeWish.UserService.Interfaces.MessageBus;
using MakeWish.UserService.Models.Events;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Events;

public sealed class UserCreatedHandler(IMessagePublisher messagePublisher) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent @event, CancellationToken cancellationToken)
    {
        var message = new UserCreatedMessage(
            new UserCreatedMessagePayload
            {
                Id = @event.User.Id,
                Name = @event.User.Name,
                Surname = @event.User.Surname
            });

        await messagePublisher.PublishAsync(message);
    }
}