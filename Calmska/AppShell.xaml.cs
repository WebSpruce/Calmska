using Calmska.Views;
using Calmska.ViewModels;
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
            await Task.Delay(1000);

            try {
                if (Preferences.Default.ContainsKey("NavigateTo"))
                {
                    string page = Preferences.Default.Get("NavigateTo", string.Empty);
                    Preferences.Default.Remove("NavigateTo");
#if ANDROID
                    Log.Debug("NavigationDebug", $"AppShell navigating to: {page}");
#endif

                    if (!string.IsNullOrEmpty(page))
                    {
                        try {
                            await Shell.Current.GoToAsync($"{page}");
                        } catch (Exception ex) {
#if ANDROID
                            Log.Error("NavigationDebug", $"Navigation error with //: {ex.Message}");
#endif
                            try {
                                await Shell.Current.GoToAsync($"{page}");
                            } catch (Exception innerEx) {
#if ANDROID
                                Log.Error("NavigationDebug", $"Alternative navigation error: {innerEx.Message}");
#endif
                            }
                        }
                    }
                }
            } catch (Exception ex) {
#if ANDROID
                Log.Error("NavigationDebug", $"Error in CheckForNotificationNavigation: {ex.Message}");
#endif
            }
        }
    }
}
