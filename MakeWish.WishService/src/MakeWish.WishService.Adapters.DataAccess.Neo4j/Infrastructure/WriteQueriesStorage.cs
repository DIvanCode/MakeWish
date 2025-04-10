namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;

public class WriteQueriesStorage : IWriteQueriesStorage
{
    private readonly List<string> _queries = [];

    public void Add(string query) => _queries.Add(query);
    
    public IReadOnlyList<string> GetQueries() => _queries.AsReadOnly();
    
    public void Clear() => _queries.Clear();
    
    public int Count() => _queries.Count;
}