using System.Text;
using System.Text.Json;
using FluentResults;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Events;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Infrastructure;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ;

public sealed class RabbitMessageConsumer(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitConnectionOptions> options) : BackgroundService
{
    private const int ExecuteDelayMs = 1000;
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await using var asyncScope = scopeFactory.CreateAsyncScope();
        
        await using var connection = asyncScope.ServiceProvider.GetRequiredService<IRabbitPersistentConnection>();
        if (!connection.IsConnected)
        {
            await connection.ConnectAsync();
        }
        
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            options.Value.ExchangeName,
            ExchangeType.Fanout,
            durable: true,
            cancellationToken: cancellationToken);
        
        await channel.QueueDeclareAsync(
            options.Value.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
        
        await channel.QueueBindAsync(
            options.Value.QueueName,
            options.Value.ExchangeName,
            routingKey: "",
            cancellationToken: cancellationToken);
        
        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (_, @event) =>
        {
            var scope = scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            var payload = Encoding.UTF8.GetString(@event.Body.ToArray());

            if (@event.BasicProperties.Headers?.TryGetValue("Type", out var type) ?? false)
            {
                INotification? notification = Encoding.UTF8.GetString(type as byte[] ?? []) switch
                {
                    "user.created" => JsonSerializer.Deserialize<UserCreatedEvent>(payload)!.ToNotification(),
                    "user.deleted" => JsonSerializer.Deserialize<UserDeletedEvent>(payload)!.ToNotification(),
                    "friendship.confirmed" => JsonSerializer.Deserialize<FriendshipConfirmedEvent>(payload)!.ToNotification(),
                    "friendship.removed" => JsonSerializer.Deserialize<FriendshipRemovedEvent>(payload)!.ToNotification(),
                    _ => null
                };

                if (notification is not null)
                {
                    await mediator.Publish(notification, cancellationToken);
                }
            }
            
            await channel.BasicAckAsync(@event.DeliveryTag, false, cancellationToken);
        };

        await channel.BasicConsumeAsync(
            options.Value.QueueName,
            false,
            "",
            false,
            false,
            null,
            consumer,
            cancellationToken
        );

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(ExecuteDelayMs), cancellationToken);
        }
    }
}