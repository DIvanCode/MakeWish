namespace MakeWish.UserService.Adapters.MessageBus.RabbitMQ.Options;

public sealed record RabbitConnectionOptions
{
    public const string SectionName = "RabbitConnection";

    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string VirtualHost { get; init; }
    public required int RetryCount { get; init; }
    public required string ExchangeName { get; init; } 
}