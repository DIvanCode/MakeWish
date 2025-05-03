using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Infrastructure;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;
using MakeWish.UserService.Interfaces.MessageBus;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ;

public sealed class RabbitMessagePublisher(
    IRabbitPersistentConnection connection,
    IOptions<RabbitConnectionOptions> options)
    : IMessagePublisher
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        IncludeFields = true
    };
    
    public async Task PublishAsync(Message message, CancellationToken cancellationToken)
    {
        if (!connection.IsConnected)
        {
            await connection.ConnectAsync(cancellationToken);
        }

        await using var channel = await connection.CreateChannelAsync(cancellationToken);
        
        await channel.ExchangeDeclareAsync(
            options.Value.ExchangeName,
            ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);
        
        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Headers = new Dictionary<string, object>
            {
                ["Type"] = message.Type
            }!
        };

        var payload = SerializePayload(message.Payload);
        
        await Policy
            .Handle<BrokerUnreachableException>().Or<SocketException>()
            .WaitAndRetryAsync(options.Value.RetryCount, k => TimeSpan.FromSeconds(Math.Pow(2, k)))
            .ExecuteAsync(async () =>
            {
                // ReSharper disable once AccessToDisposedClosure
                await channel.BasicPublishAsync(
                    exchange: options.Value.ExchangeName,
                    routingKey: "",
                    mandatory: false,
                    basicProperties: properties,
                    body: payload,
                    cancellationToken: cancellationToken);
            });
    }

    private static byte[] SerializePayload(object payload)
    {
        var json = JsonSerializer.Serialize(payload, payload.GetType(), JsonSerializerOptions);
        return Encoding.UTF8.GetBytes(json);
    }
}