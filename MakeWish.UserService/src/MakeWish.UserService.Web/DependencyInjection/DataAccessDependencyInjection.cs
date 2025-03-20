using MakeWish.UserService.Adapters.DataAccess.InMemory;
using MakeWish.UserService.Interfaces.DataAccess;

namespace MakeWish.UserService.Web.DependencyInjection;

internal static class DataAccessDependencyInjection
{
    internal static void SetupDataAccess(this IServiceCollection services)
    {
        SetupUnitOfWork(services);
        SetupRepositories(services);
    }

    private static void SetupUnitOfWork(this IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWork, UnitOfWorkStub>();
    }

    private static void SetupRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUsersRepository, UsersRepositoryStub>();
        services.AddSingleton<IFriendshipsRepository, FriendshipsRepositoryStub>();
    }
}