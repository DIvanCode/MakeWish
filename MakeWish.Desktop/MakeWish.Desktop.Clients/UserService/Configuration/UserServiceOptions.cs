namespace MakeWish.Desktop.Clients.UserService.Configuration;

internal sealed record UserServiceOptions
{
    public const string SectionName = "UserService";
    
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    
    public string BaseUrl => $"http://{Host}:{Port}";
} 