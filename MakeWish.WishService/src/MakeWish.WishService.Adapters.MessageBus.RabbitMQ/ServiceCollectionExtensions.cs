using EnsureThat;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Infrastructure;
using MakeWish.WishService.Adapters.MessageBus.RabbitMQ.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MakeWish.WishService.Adapters.MessageBus.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static void SetupMessageBusRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        var optionsSection = configuration.GetSection(RabbitConnectionOptions.SectionName);
        services.Configure<RabbitConnectionOptions>(optionsSection);
        
        var options = optionsSection.Get<RabbitConnectionOptions>();
        EnsureArg.IsNotNull(options, nameof(options));
        
        var username = Uri.EscapeDataString(options.Username);
        var password = Uri.EscapeDataString(options.Password);
        var connectionString = $"amqp://{username}:{password}@{options.Host}:{options.Port}/{options.VirtualHost}";

        services.AddScoped<IConnectionFactory>(_ => new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        });

        services.AddScoped<IRabbitPersistentConnection, RabbitPersistentConnection>();
        services.AddHostedService<RabbitMessageConsumer>();
    }
}