using System.Diagnostics;
using Android.Content;
using Calmska.Platforms.Android.ForegroundServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Plugin.Maui.Audio;

namespace Calmska.Services;

public partial class PomodoroTimerService : ObservableObject, IDisposable
{
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormattedTime), nameof(ProgressValue))]
        private int _timeRemaining;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimeType), nameof(ProgressValue))]
        private bool _isWorkSession = true;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TimeType))]
        private bool _isRunning;

        public int WorkTime { get; private set; } = 1500; // 25 minutes
        public int BreakTime { get; private set; } = 300;  // 5 minutes

        public string FormattedTime => $"{TimeRemaining / 60:D2}:{TimeRemaining % 60:D2}";
        public string TimeType => IsRunning ? (IsWorkSession ? "WORKING TIME" : "BREAK TIME") : "PAUSED";
        public float ProgressValue => TimeRemaining == 0 ? 1f : 1f - ((float)TimeRemaining / (IsWorkSession ? WorkTime : BreakTime));

        private CancellationTokenSource _cancellationTokenSource;
        private readonly IAudioManager _audioManager;
        private Task _timerTask;

        public PomodoroTimerService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            TimeRemaining = WorkTime;
        }

        public void UpdateSettings(int workTime, int breakTime)
        {
            WorkTime = workTime;
            BreakTime = breakTime;
            // If the timer isn't running, update the display
            if (!IsRunning)
            {
                TimeRemaining = IsWorkSession ? WorkTime : BreakTime;
            }
        }

        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;
#if ANDROID
            var intent = new Intent(Android.App.Application.Context, typeof(PomodoroTimeNotificationService));
            intent.SetAction("START");
            Android.App.Application.Context.StartForegroundService(intent);
#endif
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            _timerTask = RunTimerAsync(_cancellationTokenSource.Token);
        }

        public void Pause()
        {
            if (!IsRunning) return;

            IsRunning = false;
            _cancellationTokenSource?.Cancel();
#if ANDROID
            var intent = new Intent(Android.App.Application.Context, typeof(PomodoroTimeNotificationService));
            intent.SetAction("STOP");
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }

        public void Reset()
        {
            Pause();
            IsWorkSession = true;
            TimeRemaining = WorkTime;
        }

        private async Task RunTimerAsync(CancellationToken token)
        {
            try
            {
                while (IsRunning && !token.IsCancellationRequested)
                {
                    await Task.Delay(1000, token);
                    if (token.IsCancellationRequested) return;

                    TimeRemaining--;
                
                    if (TimeRemaining <= 0)
                    {
                        IsWorkSession = !IsWorkSession;
                        TimeRemaining = IsWorkSession ? WorkTime : BreakTime;
                        await PlaySoundAsync();
                    }
                }
            }
            catch (OperationCanceledException) {}
            catch (Exception ex) { Debug.WriteLine($"Timer Error: {ex.Message}"); }
        }

        private async Task PlaySoundAsync()
        {
            try
            {
                string soundFile = IsWorkSession ? "work.mp3" : "break.mp3";
                var player = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync(soundFile));
                player.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to play sound: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
}