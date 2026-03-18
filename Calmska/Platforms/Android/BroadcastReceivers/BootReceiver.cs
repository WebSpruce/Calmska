using Android.App;
using Android.Content;
using Android.Runtime;

namespace Calmska.Platforms.Android.BroadcastReceivers
{
    [BroadcastReceiver(
        Name = "com.companyname.calmska.BootReceiver",
        Enabled = true, Exported = true)]
    [IntentFilter(new[] { "android.intent.action.BOOT_COMPLETED" })]
    [Preserve(AllMembers = true)]
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
