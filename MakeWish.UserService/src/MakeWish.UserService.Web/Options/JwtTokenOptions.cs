namespace MakeWish.UserService.Web.Options;

public sealed record JwtTokenOptions
{
    public const string SectionName = "JwtToken";
    
    public string SecretKey { get; init; } = string.Empty;
    
    public int ExpiresIntervalHours { get; init; } = 1;
    
    public int RefreshIntervalHours { get; init; } = 1;    
}