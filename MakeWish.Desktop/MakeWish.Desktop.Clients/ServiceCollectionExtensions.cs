using MakeWish.Desktop.Clients.Common.UserContext;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.Desktop.Clients;

public static class ServiceCollectionExtensions
{
    public static void AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUserContext, UserContext>();
        
        services.Configure<UserServiceOptions>(configuration.GetSection(UserServiceOptions.SectionName));
        services.AddSingleton<IUserServiceClient, UserServiceClient>();
        
        services.Configure<WishServiceOptions>(configuration.GetSection(WishServiceOptions.SectionName));
        services.AddSingleton<IWishServiceClient, WishServiceClient>();
        
        services.AddHttpClient();
    }
}