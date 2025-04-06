using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;

namespace Calmska.Platforms.Android.ForegroundServices
{
    [Service(Exported = true, ForegroundServiceType = ForegroundService.TypeDataSync)]
    internal class MoodNotificationService : Service
    {
        private const int _notificationId = 1001;
        private const string _channelId = "MoodNotificationChannel";

        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartTemporaryForeground();

            NotificationScheduler.ScheduleDailyNotification(this);

            StopForeground(true);
            StopSelf();

            return StartCommandResult.NotSticky;
        }

        private void StartTemporaryForeground()
        {
            var notification = new NotificationCompat.Builder(this, _channelId)
                .SetContentTitle("Mood Reminder Active")
                .SetContentText("Daily mood reminder set.")
                .SetSmallIcon(Resource.Drawable.calmska)
                .SetPriority(NotificationCompat.PriorityMin)
                .SetOngoing(true)
                .Build();

            StartForeground(_notificationId, notification);
        }
        public override IBinder? OnBind(Intent? intent) => null;
    }
}
