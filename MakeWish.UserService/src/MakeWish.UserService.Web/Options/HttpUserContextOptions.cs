namespace MakeWish.UserService.Web.Options;

public sealed record HttpUserContextOptions
{
    public const string SectionName = "HttpUserContext";
    
    public string IdClaimType { get; init; } = string.Empty;
    public string IsAdminClaimType { get; init; } = string.Empty;
    public Guid AdminId { get; init; } = Guid.Empty;
}