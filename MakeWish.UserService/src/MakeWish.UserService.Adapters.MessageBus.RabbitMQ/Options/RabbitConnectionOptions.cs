namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;

public sealed record RabbitConnectionOptions
{
    public const string SectionName = "RabbitConnection";

    public required string ConnectionString { get; init; }
    public required int RetryCount { get; init; }
    public required string ExchangeName { get; init; } 
}