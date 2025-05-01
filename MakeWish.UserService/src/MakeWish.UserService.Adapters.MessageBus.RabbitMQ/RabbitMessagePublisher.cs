using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Infrastructure;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;
using MakeWish.UserService.Interfaces.MessageBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ;

public sealed class RabbitMessagePublisher(
    IRabbitPersistentConnection connection,
    IOptions<RabbitConnectionOptions> options)
    : IMessagePublisher
{
    public async Task PublishAsync(Message message)
    {
        if (!connection.IsConnected)
        {
            await connection.ConnectAsync();
        }

        await using var channel = await connection.CreateChannelAsync();
        
        var properties = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            Headers = new Dictionary<string, object>
            {
                ["Type"] = message.Type
            }!
        };

        var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message.Payload));
        
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
                    body: payload);
            });
    }
}