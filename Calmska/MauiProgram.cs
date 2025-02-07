using Calmska.Controls;
using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using Calmska.Services.Services;
using Calmska.ViewModels;
using Calmska.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Calmska
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
                    fonts.AddFont("CanelaBlack.otf", "CanelaBlack");
                    fonts.AddFont("CanelaLight.otf", "CanelaLight");
                    fonts.AddFont("CanelaMedium.otf", "CanelaMedium");
                    fonts.AddFont("CanelaRegular.otf", "CanelaRegular");
                    fonts.AddFont("MADEINFINITYBlack.otf", "MADEINFINITYBlack");
                    fonts.AddFont("MADEINFINITYLight.otf", "MADEINFINITYLight");
                    fonts.AddFont("MADEINFINITYMedium.otf", "MADEINFINITYMedium");
                    fonts.AddFont("MADEINFINITYRegular.otf", "MADEINFINITYRegular");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddView<LoginPage, LoginViewModel>(ServiceLifetime.Singleton);
            builder.Services.AddView<RegisterPage, RegisterViewModel>(ServiceLifetime.Singleton);
            builder.Services.AddTransient<PomodoroPage>();
            builder.Services.AddTransient<PomodoroViewModel>();
            builder.Services.AddView<TipsPage, TipsViewModel>(ServiceLifetime.Transient);
            builder.Services.AddView<SettingsPage, SettingsViewModel>(ServiceLifetime.Transient);
            builder.Services.AddSingleton<CustomTabBar>();
            builder.Services.AddSingleton<CustomTabBarViewModel>();

            builder.AddAppSettings();

            var apiBaseUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? string.Empty;
            builder.Services.AddHttpClient<IAccountService, AccountsService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            return builder.Build();
        }
        public static void AddView<TView, TViewModel>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TView : ContentPage
            where TViewModel : class
        {
            // Register the ViewModel with the specified lifetime
            var viewModelDescriptor = new ServiceDescriptor(typeof(TViewModel), typeof(TViewModel), lifetime);
            services.Add(viewModelDescriptor);

            // Register the View with the appropriate BindingContext setup
            services.Add(new ServiceDescriptor(typeof(TView), serviceProvider =>
            {
                var viewModel = serviceProvider.GetRequiredService<TViewModel>();
                var view = Activator.CreateInstance<TView>();
                view.BindingContext = viewModel;
                return view;
            }, lifetime));
        }

        private static void AddAppSettings(this MauiAppBuilder builder)
        {
            using Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("Calmska.appsettings.json");

            if (stream is not null)
            {
                IConfigurationRoot config = new ConfigurationBuilder()
                    .AddJsonStream(stream)
                    .Build();
                    builder.Configuration.AddConfiguration(config);
            }
        }

    }
}
