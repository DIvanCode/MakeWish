namespace MakeWish.WishService.Web.Options;

public sealed record HttpUserContextOptions
{
    public const string SectionName = "HttpUserContext";
    
    public string IdClaimType { get; init; } = string.Empty;
}