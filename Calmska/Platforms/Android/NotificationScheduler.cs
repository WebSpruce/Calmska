using Android.App;
using Android.Content;
using Android.Icu.Util;
using Calmska.Platforms.Android.BroadcastReceivers;

namespace Calmska.Platforms.Android
{
    internal class NotificationScheduler
    {
        public static void ScheduleDailyNotification(Context context)
        {
            int hour = Preferences.Default.Get("NotificationHour", 8);
            int minute = Preferences.Default.Get("NotificationMinute", 0);

            var calendar = Calendar.Instance;
            calendar.Set(CalendarField.HourOfDay, hour);
            calendar.Set(CalendarField.Minute, minute);
            calendar.Set(CalendarField.Second, 0);

            if (calendar.TimeInMillis < Java.Lang.JavaSystem.CurrentTimeMillis())
            {
                calendar.Add(CalendarField.DayOfYear, 1);
            }

            var intent = new Intent(context, typeof(MoodNotificationReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(
                context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.SetExactAndAllowWhileIdle(
                AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);
        }

        public static void CancelNotification(Context context)
        {
            var intent = new Intent(context, typeof(MoodNotificationReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(
                context, 0, intent, PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable);

            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);
        }
    }
}
