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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // ===== Services =====
            builder.Services.AddSingleton<IAuthService, MockAuthService>(); // TODO: swap to real AuthService when API is ready
            builder.Services.AddSingleton<ICycleService, MockCycleService>();
            builder.Services.AddSingleton<IExpenseService, MockExpenseService>();

            // ===== ViewModels =====
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<VerifyCodeViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<AddExpenseViewModel>();

            // ===== Views =====
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<VerifyCodePage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<AddExpensePage>();
            builder.Services.AddSingleton<IIncomeService, MockIncomeService>();
            builder.Services.AddTransient<AddIncomeViewModel>();
            builder.Services.AddTransient<AddIncomePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}