using System.IO;
using System.Windows;
using MakeWish.Desktop.Clients.Common.UserContext;
using Microsoft.Extensions.DependencyInjection;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.ViewModels;
using MakeWish.Desktop.Views;
using MakeWish.Desktop.Clients.UserService;
using Microsoft.Extensions.Configuration;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Configuration;
using MakeWish.Desktop.ViewModels.Wishes;
using MakeWish.Desktop.Views.Users;
using MakeWish.Desktop.Views.Wishes;
using FriendsPendingFromUserViewModel = MakeWish.Desktop.ViewModels.Users.FriendsPendingFromUserViewModel;
using FriendsPendingToUserViewModel = MakeWish.Desktop.ViewModels.Users.FriendsPendingToUserViewModel;
using FriendsViewModel = MakeWish.Desktop.ViewModels.Users.FriendsViewModel;
using LoginViewModel = MakeWish.Desktop.ViewModels.Users.LoginViewModel;
using RegisterViewModel = MakeWish.Desktop.ViewModels.Users.RegisterViewModel;
using UserViewModel = MakeWish.Desktop.ViewModels.Users.UserViewModel;

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

        services.AddSingleton<MainWindow>();
        
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogService, DialogService>();
        
        services.AddSingleton<IUserContext, UserContext>();
        services.AddHttpClient();
        
        ConfigureUsers(services, configuration);
        ConfigureWishes(services, configuration);
    }

    private static void ConfigureUsers(IServiceCollection services, IConfiguration configuration)
    {
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
        
        services.Configure<UserServiceOptions>(configuration.GetSection(UserServiceOptions.SectionName));
        services.AddSingleton<IUserServiceClient, UserServiceClient>();
    }

    private static void ConfigureWishes(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<WishView>();
        services.AddTransient<WishesView>();
        services.AddTransient<WishListsView>();

        services.AddTransient<WishViewModel>();
        services.AddTransient<WishesViewModel>();
        services.AddTransient<WishListsViewModel>();
        
        services.Configure<WishServiceOptions>(configuration.GetSection(WishServiceOptions.SectionName));
        services.AddSingleton<IWishServiceClient, WishServiceClient>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainViewModel>();
        mainWindow.Show();
    }
}