using System.IO;
using System.Windows;
using MakeWish.Desktop.Clients;
using MakeWish.Desktop.Clients.Common.UserContext;
using Microsoft.Extensions.DependencyInjection;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.Clients.UserService;
using Microsoft.Extensions.Configuration;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Configuration;

namespace MakeWish.Desktop;

public partial class App
{
    private readonly ServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IOverlayService, OverlayService>();
        services.AddSingleton<IDialogService, DialogService>();
        
        services.AddSingleton<ILoadingService, LoadingService>();
        services.AddTransient<IAsyncExecutor, AsyncExecutor>();

        services.AddClients(configuration);
        
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowView>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}