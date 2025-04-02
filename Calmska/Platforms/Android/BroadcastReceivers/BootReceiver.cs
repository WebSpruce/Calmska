using Android.App;
using Android.Content;
using Calmska.Platforms.Android.ForegroundServices;
namespace Calmska.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent?.Action != Intent.ActionBootCompleted)
                return;

            var moodNotificationService = new Intent(context, typeof(MoodNotificationService));
            context.StartForegroundService(moodNotificationService); // Reschedule daily notification
        }
    }
}
