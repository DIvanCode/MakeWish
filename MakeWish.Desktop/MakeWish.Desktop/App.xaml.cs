using System.IO;
using System.Windows;
using MakeWish.Desktop.Cards.Users;
using MakeWish.Desktop.Cards.Wishes;
using MakeWish.Desktop.Clients.Common.UserContext;
using Microsoft.Extensions.DependencyInjection;
using MakeWish.Desktop.Services;
using MakeWish.Desktop.Clients.UserService;
using Microsoft.Extensions.Configuration;
using MakeWish.Desktop.Clients.UserService.Configuration;
using MakeWish.Desktop.Clients.WishService;
using MakeWish.Desktop.Clients.WishService.Configuration;
using MakeWish.Desktop.Forms.Users;
using MakeWish.Desktop.Pages.Users;
using MakeWish.Desktop.Pages.Wishes;
using MakeWish.Desktop.Windows;

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
        services.AddTransient<IRequestExecutor, RequestExecutor>();
        services.AddSingleton<IUserContext, UserContext>();
        
        services.AddHttpClient();
        
        services.Configure<UserServiceOptions>(configuration.GetSection(UserServiceOptions.SectionName));
        services.AddSingleton<IUserServiceClient, UserServiceClient>();
        
        services.Configure<WishServiceOptions>(configuration.GetSection(WishServiceOptions.SectionName));
        services.AddSingleton<IWishServiceClient, WishServiceClient>();
        
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowView>();
        
        services.AddTransient<LoginForm>();
        services.AddTransient<LoginFormView>();
        
        services.AddTransient<RegisterForm>();
        services.AddTransient<RegisterFormView>();
        
        services.AddTransient<ProfilePage>();
        services.AddTransient<ProfilePageView>();
        
        services.AddTransient<UserCard>();
        services.AddTransient<UserCardView>();
        
        services.AddTransient<ConfirmedFriendsPage>();
        services.AddTransient<ConfirmedFriendsPageView>();
        
        services.AddTransient<PendingToUserFriendsPage>();
        services.AddTransient<PendingToUserFriendsPageView>();
        
        services.AddTransient<PendingFromUserFriendsPage>();
        services.AddTransient<PendingFromUserFriendsPageView>();
        
        services.AddTransient<SearchUserForm>();
        services.AddTransient<SearchUserFormView>();
        
        services.AddTransient<UserWishesPage>();
        services.AddTransient<UserWishesPageView>();
        
        services.AddTransient<UserWishListsPage>();
        services.AddTransient<UserWishListsPageView>();
        
        services.AddTransient<UserPromisedWishesPage>();
        services.AddTransient<UserPromisedWishesPageView>();

        services.AddTransient<WishPage>();
        services.AddTransient<WishPageView>();
        
        services.AddTransient<WishCard>();
        services.AddTransient<WishCardView>();
        
        services.AddTransient<WishListPage>();
        services.AddTransient<WishListPageView>();
        
        services.AddTransient<WishListCard>();
        services.AddTransient<WishListCardView>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = _serviceProvider.GetRequiredService<MainWindowView>();
        mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}