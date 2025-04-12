using Calmska.Views;
using Calmska.ViewModels;
using Microsoft.Maui.Controls.PlatformConfiguration;
#if ANDROID
using Android.Util;
#endif

namespace Calmska
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute("loginpage", typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(PomodoroPage), typeof(PomodoroPage));
            Routing.RegisterRoute(nameof(TipsPage), typeof(TipsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(TipsListPage), typeof(TipsListPage));
            Routing.RegisterRoute("moodentrypage", typeof(MoodEntryPage));

            MainThread.BeginInvokeOnMainThread(CheckForNotificationNavigation);
        }
        private async void CheckForNotificationNavigation()
        {
            await Task.Delay(500); // Small delay to allow initialization

            if (Preferences.Default.ContainsKey("NavigateTo"))
            {
                string page = Preferences.Default.Get("NavigateTo", string.Empty);
                Preferences.Default.Remove("NavigateTo");
#if ANDROID
                Android.Util.Log.Debug("NavigationDebug", $"Navigating to: {page}");
#endif

                if (!string.IsNullOrEmpty(page))
                {
                    await Shell.Current.GoToAsync($"{page}");
                }
            }
        }
    }
}
