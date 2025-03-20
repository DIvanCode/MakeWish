using System.Reflection;
using MakeWish.UserService.UseCases.Services;

namespace MakeWish.UserService.Web.DependencyInjection;

internal static class UseCasesDependencyInjection
{
    internal static void SetupUseCases(this IServiceCollection services)
    {
        SetupMediator(services);
        SetupPasswordService(services);
    }

    private static void SetupMediator(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    private static void SetupPasswordService(this IServiceCollection services)
    {
        services.AddTransient<IPasswordService, BCryptPasswordService>();   
    }
}