using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.ViewModels;
using MakeWish.Desktop.Views;
using MakeWish.Desktop.Clients.UserService;
using MakeWish.Desktop.Clients.UserContext;
using Microsoft.Extensions.Configuration;
using MakeWish.Desktop.Clients.UserService.Configuration;

namespace MakeWish.Desktop;

public partial class App : Application
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<MainWindow>();
        services.AddTransient<LoginView>();
        services.AddTransient<RegisterView>();
        services.AddTransient<UserView>();
        services.AddTransient<FriendsView>();
        services.AddTransient<FriendsPendingFromUserView>();
        services.AddTransient<FriendsPendingToUserView>();
        
        services.AddTransient<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<UserViewModel>();
        services.AddTransient<FriendsViewModel>();
        services.AddTransient<FriendsPendingFromUserViewModel>();
        services.AddTransient<FriendsPendingToUserViewModel>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<IUserServiceClient, UserServiceClient>();
        services.AddSingleton<IUserContext, UserContext>();
        
        services.Configure<UserServiceOptions>(configuration.GetSection(UserServiceOptions.SectionName));
        
        services.AddHttpClient();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }
}