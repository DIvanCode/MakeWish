using MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;
using MakeWish.WishService.Interfaces.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j;

public sealed class UnitOfWork(
    IUsersRepository usersRepository,
    IWishesRepository wishesRepository,
    IWishListsRepository wishListsRepository,
    IServiceProvider serviceProvider,
    IWriteQueriesStorage writeQueriesStorage) : IUnitOfWork
{
    public IUsersRepository Users { get; } = usersRepository;
    public IWishesRepository Wishes { get; } = wishesRepository;
    public IWishListsRepository WishLists { get; } = wishListsRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        await using var connection = serviceProvider.GetRequiredService<Neo4jConnection>();

        var queriesCount = writeQueriesStorage.Count();
        
        await connection.BeginTransactionAsync();
        try
        {
            foreach (var query in writeQueriesStorage.GetQueries())
            {
                await connection.ExecuteWriteQueryAsync(query);
            }

            await connection.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await connection.RollbackTransactionAsync();
            throw;
        }

        writeQueriesStorage.Clear();
        return queriesCount;
    }
}