using System.Net.Sockets;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Infrastructure;

public sealed class RabbitPersistentConnection(
    IConnectionFactory connectionFactory,
    IOptions<RabbitConnectionOptions> options)
    : IRabbitPersistentConnection
{
    private IConnection? _connection;
    private bool _disposed;

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
        }
        
        return await _connection!.CreateChannelAsync(cancellationToken: cancellationToken);
    }
    
    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await Policy
            .Handle<SocketException>().Or<BrokerUnreachableException>()
            .WaitAndRetryAsync(options.Value.RetryCount, k => TimeSpan.FromSeconds(Math.Pow(2, k)))
            .ExecuteAsync(async () => _connection = await connectionFactory.CreateConnectionAsync(cancellationToken));
    }

    public async ValueTask DisposeAsync()
    {
        if (!IsConnected)
        {
            return;
        }

        await _connection!.CloseAsync();
        await _connection!.DisposeAsync();
        _disposed = true;
    }
}