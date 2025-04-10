namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Options;

// ReSharper disable once InconsistentNaming
public sealed record DbConnectionOptions
{
    public const string SectionName = "DbConnection";

    public string Uri { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}