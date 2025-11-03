using Calmska.Controls;
using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using Calmska.Services.Services;
using Calmska.ViewModels;
using Calmska.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Calmska.Services;
using Plugin.Maui.Audio;

namespace Calmska
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }
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
                    fonts.AddFont("MaterialIconsRoundRegular.otf", "MaterialIcons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<PomodoroTimerService>();
            
            builder.Services.AddView<LoginPage, LoginViewModel>(ServiceLifetime.Singleton);
            builder.Services.AddView<RegisterPage, RegisterViewModel>(ServiceLifetime.Singleton);
            builder.Services.AddView<TipsPage, TipsViewModel>(ServiceLifetime.Transient);
            builder.Services.AddView<SettingsPage, SettingsViewModel>(ServiceLifetime.Transient);
            builder.Services.AddView<TipsListPage, TipsListViewModel>(ServiceLifetime.Transient);
            builder.Services.AddView<MoodEntryPage, MoodEntryPageViewModel>(ServiceLifetime.Transient);

            builder.Services.AddTransient<PomodoroPage>();
            builder.Services.AddSingleton<CustomTabBar>();

            builder.Services.AddSingleton<CustomTabBarViewModel>();
            builder.Services.AddTransient<PomodoroViewModel>();


            builder.AddAppSettings();

            var apiBaseUrl = builder.Configuration.GetValue<string>("ApiUrl") ?? string.Empty;
            builder.Services.AddHttpClient<IAccountService, AccountsService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            builder.Services.AddHttpClient<IService<SettingsDTO>, SettingsService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            builder.Services.AddHttpClient<IService<TipsDTO>, TipsService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            builder.Services.AddHttpClient<ITypesService<Types_TipsDTO>, TypesTipsService>(client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
            });
            builder.Services.AddHttpClient<IService<MoodDTO>, MoodService>(client =>
                client.BaseAddress = new Uri(apiBaseUrl)
            );
            builder.Services.AddHttpClient<IService<MoodHistoryDTO>, MoodHistoryService>(client =>
                client.BaseAddress = new Uri(apiBaseUrl)
            );
            
            var app = builder.Build(); // Build the app

            Services = app.Services; // <-- ASSIGN THE SERVICE PROVIDER

            return app;
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
