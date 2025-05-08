namespace MakeWish.Desktop.Clients.WishService.Configuration;

public sealed record WishServiceOptions
{
    public const string SectionName = "WishService";
    
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    
    public string BaseUrl => $"http://{Host}:{Port}";
} 