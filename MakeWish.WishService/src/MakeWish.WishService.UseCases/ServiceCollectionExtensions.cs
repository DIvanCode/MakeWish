using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.WishService.UseCases;

public static class ServiceCollectionExtensions
{
    public static void SetupUseCases(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}