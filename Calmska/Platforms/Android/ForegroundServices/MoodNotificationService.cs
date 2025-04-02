using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Icu.Util;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;
using Calmska.Platforms.Android.BroadcastReceivers;

namespace Calmska.Platforms.Android.ForegroundServices
{
    [Service(Exported = true, ForegroundServiceType = ForegroundService.TypeDataSync)]
    internal class MoodNotificationService : Service
    {
        private const int _notificationId = 1001;
        private const string _channelId = "MoodNotificationChannel";

        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartForegroundServiceWithNotification();

            ScheduleDailyNotification();

            return StartCommandResult.Sticky;
        }

        private void StartForegroundServiceWithNotification()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.PutExtra("NavigateTo", "MoodEntryPage");
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(
                this,
                0,
                notificationIntent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var notification = new NotificationCompat.Builder(this, _channelId)
                .SetContentTitle("Mood Reminder")
                .SetContentText("Tap to record your mood")
                .SetSmallIcon(Resource.Drawable.calmska)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetContentIntent(pendingIntent)
                .SetOngoing(true)
                .Build();

            StartForeground(_notificationId, notification);
        }

        private void ScheduleDailyNotification()
        {
            var alarmManager = (AlarmManager)GetSystemService(AlarmService);

            // Retrieve user-set time from preferences
            int hour = Preferences.Default.Get("NotificationHour", 8);
            int minute = Preferences.Default.Get("NotificationMinute", 0);

            var calendar = Calendar.Instance;
            calendar.Set(CalendarField.HourOfDay, hour);
            calendar.Set(CalendarField.Minute, minute);
            calendar.Set(CalendarField.Second, 0);

            // If time has already passed today, set it for tomorrow
            if (calendar.TimeInMillis < Java.Lang.JavaSystem.CurrentTimeMillis())
            {
                calendar.Add(CalendarField.DayOfYear, 1);
            }

            var intent = new Intent(this, typeof(MoodNotificationReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(
                this, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            alarmManager.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
        }

        public override IBinder? OnBind(Intent? intent) => null;
    }
}
