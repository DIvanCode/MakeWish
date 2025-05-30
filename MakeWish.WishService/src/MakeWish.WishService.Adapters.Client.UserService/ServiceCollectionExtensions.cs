using MakeWish.WishService.Adapters.Client.UserService.Configuration;
using MakeWish.WishService.Interfaces.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.WishService.Adapters.Client.UserService;

public static class ServiceCollectionExtensions
{
    public static void SetupClientUserService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UserServiceOptions>(configuration.GetSection(UserServiceOptions.SectionName));
        services.AddScoped<IUserServiceClient, UserServiceClient>();
        services.AddHttpClient();
    }
}