using Android.App;
using Android.Content;
namespace Calmska.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                NotificationScheduler.ScheduleDailyNotification(context); // re-schedule after reboot
            }
        }
    }
}
