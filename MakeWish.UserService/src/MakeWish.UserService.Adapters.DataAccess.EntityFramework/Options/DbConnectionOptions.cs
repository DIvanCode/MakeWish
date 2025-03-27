namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Options;

public sealed record DbConnectionOptions
{
    public const string SectionName = "DbConnection";
    
    public string ConnectionString { get; init; } = string.Empty;
}