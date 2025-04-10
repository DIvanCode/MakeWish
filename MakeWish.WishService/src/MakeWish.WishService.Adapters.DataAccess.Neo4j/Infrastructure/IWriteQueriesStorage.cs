namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;

public interface IWriteQueriesStorage
{
    void Add(string query);
    IReadOnlyList<string> GetQueries();
    void Clear();
    int Count();
}