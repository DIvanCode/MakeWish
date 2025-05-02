namespace MakeWish.UserService.Interfaces.MessageBus;

public interface IMessagePublisher
{
    Task PublishAsync(Message message, CancellationToken cancellationToken);
}
