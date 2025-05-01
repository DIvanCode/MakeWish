using RabbitMQ.Client;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Infrastructure;

public interface IRabbitPersistentConnection : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync();

    Task<IChannel> CreateChannelAsync();
}