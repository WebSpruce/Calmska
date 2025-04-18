using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Calmska.Platforms.Android.BroadcastReceivers;
using Intent = Android.Content.Intent;

namespace Calmska
{
    [Activity(Theme = "@style/Maui.SplashTheme", Exported = true, MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            Intent = intent;
            ProcessNotificationIntent(intent);
        }
        private MoodNotificationReceiver _receiver;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckPostNotificationPermission();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                var alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
                if (!alarmManager.CanScheduleExactAlarms())
                {
                    var intent = new Intent(Android.Provider.Settings.ActionRequestScheduleExactAlarm);
                    intent.SetFlags(ActivityFlags.NewTask);
                    StartActivity(intent);
                }
            }

            CreateNotificationChannel();
            Log.Debug("MainActivity", "OnCreate called");
            ProcessNotificationIntent(Intent);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_receiver != null)
            {
                UnregisterReceiver(_receiver);
                _receiver = null;
            }
        }
        private void CheckPostNotificationPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Android.Manifest.Permission.PostNotifications }, 0);
                }
            }
        }
        private void ProcessNotificationIntent(Intent? intent)
        {
            if (intent?.HasExtra("NavigateTo") == true)
            {
                Log.Debug("NavigationDebug", "Processing notification intent");
                string? userJson = null;
            
                try {
                    userJson = SecureStorage.Default.GetAsync("user_info").Result;
                } catch (Exception ex) {
                    Log.Error("NavigationDebug", $"Error accessing secure storage: {ex.Message}");
                }
            
                bool isLoggedIn = !string.IsNullOrEmpty(userJson);
                string page = intent.GetStringExtra("NavigateTo") ?? string.Empty;
            
                if (!string.IsNullOrEmpty(page))
                {
                    string targetPage = isLoggedIn ? page : "loginpage";
                    Log.Debug("NavigationDebug", $"Setting navigation preference to: {targetPage}");
                    
                    Preferences.Default.Set("NavigateTo", targetPage);
                    if (!isLoggedIn)
                        Preferences.Default.Set("NavigateAfterLogin", page);
                    
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try {
                            await Task.Delay(500);
                            await Shell.Current.GoToAsync(targetPage);
                            Log.Debug("NavigationDebug", $"Immediate navigation attempted to: {targetPage}");
                        } catch (Exception ex) {
                            Log.Error("NavigationDebug", $"Navigation error: {ex.Message}");
                        }
                    });
                }
            }
        }
        private async Task<bool> IsUserLoggedInAsync()
        {
            try
            {
                string? userJson = await SecureStorage.Default.GetAsync("user_info");
                return !string.IsNullOrEmpty(userJson);
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("AuthDebug", $"Error checking login state: {ex.Message}");
                return false;
            }
        }
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "MoodNotificationChannel",
                    "Mood Reminders",
                    NotificationImportance.High)
                {
                    Description = "Daily reminders to track your mood."
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }
}
