using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SmartBudget.Mobile.Helpers;
using SmartBudget.Mobile.Services;
using SmartBudget.Mobile.Services.Interfaces;
using SmartBudget.Mobile.ViewModels;
using SmartBudget.Mobile.Views;

namespace SmartBudget.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()          // <-- add this line
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();

            // Services
            builder.Services.AddSingleton<IAuthService, MockAuthService>(); // TODO: swap to real AuthService when API is ready

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();

            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
#endif

            return builder.Build();
        }
    }
}