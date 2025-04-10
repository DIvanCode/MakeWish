namespace MakeWish.WishService.Web.Options;

public sealed record JwtTokenOptions
{
    public const string SectionName = "JwtToken";
    
    public string SecretKey { get; init; } = string.Empty;
    public int RefreshIntervalHours { get; init; } = 1;    
}