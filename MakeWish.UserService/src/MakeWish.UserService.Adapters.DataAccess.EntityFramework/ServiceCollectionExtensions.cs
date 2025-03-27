using EnsureThat;
using MakeWish.UserService.Adapters.DataAccess.EntityFramework.Options;
using MakeWish.UserService.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework;

public static class ServiceCollectionExtensions
{
    public static void SetupDataAccessEntityFramework(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionOptions = configuration.GetSection(DbConnectionOptions.SectionName).Get<DbConnectionOptions>();
        EnsureArg.IsNotNull(dbConnectionOptions, nameof(dbConnectionOptions));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddDbContext<UnitOfWork>(options =>
        {
            options.UseNpgsql(
                dbConnectionOptions.ConnectionString,
                providerOptions =>
                {
                    providerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
                });
        });
    }
}