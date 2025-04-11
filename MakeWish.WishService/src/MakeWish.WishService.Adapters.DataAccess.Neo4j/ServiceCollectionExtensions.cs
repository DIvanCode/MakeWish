using MakeWish.WishService.Adapters.DataAccess.Neo4j.Infrastructure;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Mappers;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Options;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Queries;
using MakeWish.WishService.Adapters.DataAccess.Neo4j.Repositories;
using MakeWish.WishService.Interfaces.DataAccess;
using MakeWish.WishService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.WishService.Adapters.DataAccess.Neo4j;

public static class ServiceCollectionExtensions
{
    // ReSharper disable once InconsistentNaming
    public static void SetupDataAccessNeo4j(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<Neo4jConnection>();
        services.AddScoped<IWriteQueriesStorage, WriteQueriesStorage>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IWishesRepository, WishesRepository>();
        services.AddScoped<IWishListsRepository, WishListsRepository>();
        
        services.Configure<DbConnectionOptions>(configuration.GetSection(DbConnectionOptions.SectionName));

        services.AddTransient<IMapper<User>, UsersMapper>();
        services.AddTransient<IMapper<Wish>, WishesMapper>();
        services.AddTransient<IMapper<WishList>, WishListsMapper>();

        services.AddTransient<IQueryBuilder<User>, QueryBuilder<User>>();
        services.AddTransient<IQueryBuilder<Wish>, QueryBuilder<Wish>>();
        services.AddTransient<IQueryBuilder<WishList>, QueryBuilder<WishList>>();
    }
}