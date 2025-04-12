using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace Calmska.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { "com.companyname.notifTest.NOTIFICATION_CLICKED" })]
    public class MoodNotificationReceiver : BroadcastReceiver
    {
        private const string CHANNEL_ID = "MoodNotificationChannel";
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null) return;

            var notificationIntent = new Intent(context, typeof(MainActivity));
            //notificationIntent.PutExtra("OpenFromNotification", true);
            notificationIntent.PutExtra("NavigateTo", "moodentrypage");
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

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
            notificationManager.Notify(1002, notification);
        }
    }
}
