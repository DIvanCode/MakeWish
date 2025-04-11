using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j.Repositories;

public sealed class WishesRepository(IServiceProvider serviceProvider)
    : BaseRepository<Wish>(serviceProvider), IWishesRepository
{
    public override void Add(Wish wish)
    {
        base.Add(wish);

        var query = NewQuery()
            .MatchWish(wish)
            .LinkWishToOwner(wish.Owner);

        var promiser = ExtractPromiser(wish);
        if (promiser is not null)
        {
            query = query.LinkWishToPromiser(promiser);
        }
        
        var completer = ExtractCompleter(wish);
        if (completer is not null)
        {
            query = query.LinkWishToCompleter(completer);
        }
        
        AddWriteQuery(query.Build());
    }
    
    public override void Update(Wish wish)
    {
        base.Update(wish);

        var query = NewQuery().MatchWish(wish);
        
        var promiser = ExtractPromiser(wish);
        query = promiser is null ? query.UnlinkWishFromPromiser() : query.LinkWishToPromiser(promiser);
        
        var completer = ExtractCompleter(wish);
        query = completer is null ? query.UnlinkWishFromCompleter() : query.LinkWishToCompleter(completer);
        
        AddWriteQuery(query.Build());
    }
    
    public async Task<Wish?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWish(id)
            .MatchWishOwner()
            .OptionalMatchWishPromiser()
            .OptionalMatchWishCompleter()
            .Build();
        var result = await ExecuteAsync(query, cancellationToken);
        return result.SingleOrDefault();
    }
    
    public async Task<List<Wish>> GetWithPromiserAsync(User promiser, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWish()
            .MatchWishOwner()
            .MatchWishPromiser(promiser)
            .OptionalMatchWishCompleter()
            .Build();
        var result = await ExecuteAsync(query, cancellationToken);
        return result.ToList();
    }

    public async Task<List<Wish>> GetWithStatusAndOwnerAsync(WishStatus status, User owner, CancellationToken cancellationToken)
    {
        var query = NewQuery()
            .MatchWish(status)
            .MatchWishOwner(owner)
            .OptionalMatchWishPromiser()
            .OptionalMatchWishCompleter()
            .Build();
        var result = await ExecuteAsync(query, cancellationToken);
        return result.ToList();
    }
    
    private static User? ExtractPromiser(Wish wish)
        => (User?)ReflectionHelper<Wish>.GetPropertyValue(wish, WishesMapper.PromiserProperty);
    
    private static User? ExtractCompleter(Wish wish)
        => (User?)ReflectionHelper<Wish>.GetPropertyValue(wish, WishesMapper.CompleterProperty);
}