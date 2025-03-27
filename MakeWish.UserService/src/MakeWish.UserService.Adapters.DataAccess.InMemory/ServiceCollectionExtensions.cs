using MakeWish.UserService.Interfaces.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.UserService.Adapters.DataAccess.InMemory;

public static class ServiceCollectionExtensions
{
    internal static void SetupDataAccessInMemory(this IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWork, UnitOfWorkStub>();
        services.AddSingleton<IUsersRepository, UsersRepositoryStub>();
        services.AddSingleton<IFriendshipsRepository, FriendshipsRepositoryStub>();
    }
}