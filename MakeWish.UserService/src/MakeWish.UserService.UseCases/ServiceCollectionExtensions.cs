using System.Reflection;
using MakeWish.UserService.UseCases.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MakeWish.UserService.UseCases;

public static class ServiceCollectionExtensions
{
    public static void SetupUseCases(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddTransient<IPasswordService, BCryptPasswordService>();
    }
}