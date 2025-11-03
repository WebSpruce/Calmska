using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using Calmska.Services;

namespace Calmska.Platforms.Android.ForegroundServices;

[Service(Exported = true, ForegroundServiceType = ForegroundService.TypeShortService)]
public class PomodoroTimeNotificationService : Service
{
    private const int _notificationId = 1002; 
    private const string _channelId = "PomodoroTimeNotificationChannel";
    private NotificationManagerCompat _notificationManager;
    private PomodoroTimerService _timerService;
    public override IBinder? OnBind(Intent? intent) => null;

    public override void OnCreate()
    {
        base.OnCreate();
        _notificationManager = NotificationManagerCompat.From(this);
        CreateNotificationChannel();
        _timerService = MauiProgram.Services.GetRequiredService<PomodoroTimerService>();
        _timerService.PropertyChanged += OnTimerServicePropertyChanged;
    }
     private void OnTimerServicePropertyChanged(object sender, PropertyChangedEventArgs e)
     {
         if (e.PropertyName == nameof(PomodoroTimerService.TimeRemaining))
         {
             var notification = BuildNotification();
             _notificationManager.Notify(_notificationId, notification);
         }
     }

     public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
     {
         if (intent?.Action == "START")
         {
             var notification = BuildNotification();
             if (Build.VERSION.SdkInt >= BuildVersionCodes.UpsideDownCake)
             {
                 StartForeground(_notificationId, notification, ForegroundService.TypeShortService);
             }
             else
             {
                 StartForeground(_notificationId, notification);
             }
         }
         else if (intent?.Action == "STOP")
         {
             StopForeground(true);
             StopSelf();
         }
         return StartCommandResult.Sticky;
     }

     public override void OnDestroy()
     {
         if (_timerService != null)
         {
             _timerService.PropertyChanged -= OnTimerServicePropertyChanged;
         }
         base.OnDestroy();
     }

     private Notification BuildNotification()
     {
         string title = _timerService.IsWorkSession ? "Work Session" : "Break Time";
         string timeText = _timerService.FormattedTime;
         int totalSeconds = _timerService.IsWorkSession ? _timerService.WorkTime : _timerService.BreakTime;

        var intent = new Intent(this, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Immutable);

        var bigTextStyle = new NotificationCompat.BigTextStyle();
        bigTextStyle.SetSummaryText(timeText);
            
        var builder = new NotificationCompat.Builder(this, _channelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(timeText)
            .SetSmallIcon(Resource.Drawable.calmska)
            .SetProgress(totalSeconds, _timerService.TimeRemaining, false)
            .SetOngoing(true)
            .SetSilent(true)
            .SetCategory(NotificationCompat.CategoryProgress)   // sends signal to the OS that this is an important
            .SetForegroundServiceBehavior(NotificationCompat.ForegroundServiceImmediate)    // ensures that notification is visible at once
            .SetStyle(bigTextStyle)     // ensures that text is visible on all devices
            .SetOnlyAlertOnce(true);    
        return builder.Build();
     }
    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;
        
        var channel = new NotificationChannel(_channelId, "Pomodoro Timer", NotificationImportance.Low);
        channel.Description = "Shows the current Pomodoro timer progress.";
        _notificationManager.CreateNotificationChannel(channel);
    }
}