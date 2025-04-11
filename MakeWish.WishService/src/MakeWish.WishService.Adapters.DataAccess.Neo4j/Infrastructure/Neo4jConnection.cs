using MakeWish.WishService.Adapters.DataAccess.Neo4j.Options;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;

// ReSharper disable once InconsistentNaming
public sealed class Neo4jConnection : IDisposable, IAsyncDisposable
{
    private readonly IDriver _driver;
    private IAsyncSession? _session;
    private IAsyncTransaction? _transaction;

    public Neo4jConnection(IOptions<DbConnectionOptions> options)
    {
        var connectionOptions = options.Value;
        
        var uri = connectionOptions.Uri;
        var username = connectionOptions.Username;
        var password = connectionOptions.Password;

        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
    }
    
    public async Task<IReadOnlyList<IRecord>> ExecuteReadQueryAsync(string query, CancellationToken cancellationToken)
    {
        Console.WriteLine(query);
        var (result, _) = await _driver.ExecutableQuery(query).ExecuteAsync(cancellationToken);
        return result;
    }
    
    public async Task ExecuteWriteQueryAsync(string query)
    {
        if (_transaction is null)
        {
            await BeginTransactionAsync();
            try
            {
                await ExecuteWriteQueryAsync(query);
                await CommitTransactionAsync();
            }
            catch (Exception)
            {
                await RollbackTransactionAsync();
                throw;
            }
        }
        else
        {
            Console.WriteLine(query);
            await _transaction.RunAsync(query);
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }
        
        _session = _driver.AsyncSession();
        _transaction = await _session.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No transaction available.");
        }
        
        await _transaction.CommitAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No transaction available.");
        }
        
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _session?.Dispose();
        _driver.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_session is not null)
        {
            await _session.DisposeAsync();
        }

        await _driver.DisposeAsync();
    }
}