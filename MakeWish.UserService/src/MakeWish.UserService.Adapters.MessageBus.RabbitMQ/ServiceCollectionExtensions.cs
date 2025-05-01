using EnsureThat;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Infrastructure;
using MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;
using MakeWish.UserService.Interfaces.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ;

public static class ServiceCollectionExtensions
{
    public static void SetupMessageBusRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        var optionsSection = configuration.GetSection(RabbitConnectionOptions.SectionName);
        services.Configure<RabbitConnectionOptions>(optionsSection);
        
        var options = optionsSection.Get<RabbitConnectionOptions>();
        EnsureArg.IsNotNull(options, nameof(options));

        services.AddScoped<IConnectionFactory>(_ => new ConnectionFactory
        {
            Uri = new Uri(options.ConnectionString)
        });

        services.AddScoped<IRabbitPersistentConnection, RabbitPersistentConnection>();
        services.AddScoped<IMessagePublisher, RabbitMessagePublisher>();
    }
}