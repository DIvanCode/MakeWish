﻿using RabbitMQ.Client;

namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Infrastructure;

public interface IRabbitPersistentConnection : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}