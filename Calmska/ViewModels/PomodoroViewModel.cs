using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Calmska.ViewModels
{
    public partial class PomodoroViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _playPauseButtonText = "Play";
        public float ProgressValue => 1 - ((float)_timeRemaining / (_isWorkSession ? WorkTime : BreakTime));
        private static int _timeRemaining;
        public string TimeRemaining => $"{_timeRemaining / 60:D2}:{_timeRemaining % 60:D2}";

        private const int WorkTime = 10;
        private const int BreakTime = 5;
        private bool _isRunning;
        private static bool _isWorkSession = true;
        private CancellationTokenSource _cancellationTokenSource;
        public PomodoroViewModel()
        {
            //string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
            //var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
            _timeRemaining = WorkTime;
        }
        [RelayCommand]
        private async void PlayPause()
        {
            if (_isRunning)
            {   // Pause
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                PlayPauseButtonText = "Play";
            }
            else
            {   // Play
                _isRunning = true;
                PlayPauseButtonText = "Pause";
                _cancellationTokenSource = new CancellationTokenSource();
                await RunTimerAsync(_cancellationTokenSource.Token);
            }
        }

        [RelayCommand]
        private void ResetTimer()
        {
            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            _isWorkSession = true;
            _timeRemaining = WorkTime;
            PlayPauseButtonText = "Play";
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(ProgressValue));
        }

        private async Task RunTimerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_isRunning && _timeRemaining > 0)
                {
                    await Task.Delay(1000, cancellationToken);

                    if (cancellationToken.IsCancellationRequested)
                        return;

                    _timeRemaining--;
                    OnPropertyChanged(nameof(TimeRemaining));
                    OnPropertyChanged(nameof(ProgressValue));
                }

                if (_timeRemaining == 0)
                {
                    _isWorkSession = !_isWorkSession;
                    _timeRemaining = _isWorkSession ? WorkTime : BreakTime;
                } 

                _isRunning = false;
                PlayPauseButtonText = "Play";
            }
            catch (TaskCanceledException)
            {
                // Ignore: Task was canceled when pausing
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Timer Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Warning!", $"Error occurs:\n{ex.Message}", "Close");
            }
        }
    }
}
