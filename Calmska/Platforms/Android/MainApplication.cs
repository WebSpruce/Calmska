using Android.App;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace Calmska
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }
        public override void OnCreate()
        {
            base.OnCreate();
            CreateNotificationChannel();
        }
        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelId = "foreground_channel";
                var channelName = "Foreground Service";
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default)
                {
                    Description = "Channel for foreground service notifications"
                };
                var notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
        protected override MauiApp CreateMauiApp()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
            {
                h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
            });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToAndroid());
#endif
            });

            return MauiProgram.CreateMauiApp();
        }
    }
}
