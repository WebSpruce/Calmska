using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;

namespace Calmska.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(
        Name = "com.companyname.calmska.MoodNotificationReceiver",
        Enabled = true, Exported = true)]
    [IntentFilter(new[] { "com.companyname.calmska.NOTIFICATION_CLICKED" })]
    [Preserve(AllMembers = true)]
    public class MoodNotificationReceiver : BroadcastReceiver
    {
        private const string CHANNEL_ID = "MoodNotificationChannel";

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null) return;

            // Create an intent to launch MainActivity
            var notificationIntent = new Intent(context, typeof(MainActivity));
            notificationIntent.PutExtra("NavigateTo", "moodentrypage");

            // These flags ensure the app launches properly whether it's closed or in background
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NewTask);

            var pendingIntent = PendingIntent.GetActivity(
                context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var notification = new NotificationCompat.Builder(context, CHANNEL_ID)
                .SetContentTitle("How's Your Mood?")
                .SetContentText("Tap to log your mood now.")
                .SetSmallIcon(Resource.Drawable.calmska)
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetContentIntent(pendingIntent)
                .Build();

            var notificationManager = NotificationManagerCompat.From(context);
            try
            {
                notificationManager.Notify(1002, notification);
            }
            catch (Exception ex)
            {
                Log.Error("NotificationDebug", $"Failed to show notification: {ex.Message}");
            }
        }
    }
}
