using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Calmska.Platforms.Android;
using Calmska.Platforms.Android.BroadcastReceivers;

namespace Calmska
{
    [Activity(Theme = "@style/Maui.SplashTheme", Exported = true, MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);

            if (intent?.HasExtra("NavigateTo") == true)
            {
                string page = intent.GetStringExtra("NavigateTo");

                if (!string.IsNullOrEmpty(page))
                {
                    Preferences.Default.Set("NavigateTo", page);
                    NavigateToPage(page);
                }
            }
        }
        private MoodNotificationReceiver _receiver;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckPostNotificationPermission();
            CreateNotificationChannel();
            Android.Util.Log.Debug("MainActivity", "OnCreate called");

            if (Intent?.HasExtra("NavigateTo") == true)
            {
                string page = Intent.GetStringExtra("NavigateTo");
                if (!string.IsNullOrEmpty(page))
                {
                    Preferences.Default.Set("NavigateTo", page);
                    NavigateToPage(page);
                }
            }
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

        private void NavigateToPage(string page)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"{page}");
            });
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
