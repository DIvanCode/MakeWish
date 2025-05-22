using MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Repositories;

public sealed class UsersRepository(IServiceProvider serviceProvider)
    : BaseRepository<User>(serviceProvider), IUsersRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = NewQuery().MatchUser(id).Build();
        var result = await ExecuteAsync(query, cancellationToken);
        return result.SingleOrDefault();
    }
    
    public async Task<List<User>> GetUsersWithAccessToWishListAsync(WishList wishList, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWishList(wishList)
            .MatchUserAccessToWishList();
        var result = await ExecuteAsync(query.Build(), cancellationToken);
        return result.ToList();
    }
}