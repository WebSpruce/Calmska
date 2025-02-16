using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.Json;

namespace Calmska.ViewModels
{
    public partial class PomodoroViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _navBarTitle = string.Empty;
        [ObservableProperty]
        private string _playPauseIcon = IconFont.Play_arrow;
        [ObservableProperty]
        private string _timeType = "PAUSED";
        public float ProgressValue => 1 - ((float)_timeRemaining / (_isWorkSession ? WorkTime : BreakTime));
        private static int _timeRemaining;
        public string TimeRemaining => $"{_timeRemaining / 60:D2}:{_timeRemaining % 60:D2}";

        [ObservableProperty]
        private int _workTime = 10;
        [ObservableProperty]
        private int _breakTime = 5;
        private bool _isRunning;
        private static bool _isWorkSession = true;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IService<SettingsDTO> _settingsService;
        public PomodoroViewModel(IService<SettingsDTO> settingsService)
        {
            _settingsService = settingsService;
            string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
            var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
            NavBarTitle = user != null ? (user.UserName ?? string.Empty) : string.Empty;

            Task.Run(async () => await LoadTimeSettings(user));
        }
        [RelayCommand]
        private async void PlayPause()
        {
            if (_isRunning)
            {   // Pause
                _isRunning = false;
                _cancellationTokenSource?.Cancel();
                PlayPauseIcon = IconFont.Play_arrow;
                TimeType = "PAUSED";
            }
            else
            {   // Play
                _isRunning = true;
                PlayPauseIcon = IconFont.Pause;
                TimeType = "WORKING TIME";
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
            PlayPauseIcon = IconFont.Play_arrow;
            TimeType = "PAUSED";
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(ProgressValue));
        }

        private async Task RunTimerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (_isRunning)
                {
                    TimeType = _isWorkSession ? "WORKING TIME" : "BREAK TIME";
                    OnPropertyChanged(nameof(TimeType));

                    while (_isRunning && _timeRemaining > 0)
                    {
                        await Task.Delay(1000, cancellationToken);

                        if (cancellationToken.IsCancellationRequested)
                            return;

                        _timeRemaining--;
                        OnPropertyChanged(nameof(TimeRemaining));
                        OnPropertyChanged(nameof(ProgressValue));
                    }

                    // Switch to the next mode (Work → Break, Break → Work)
                    if (_timeRemaining == 0)
                    {
                        _isWorkSession = !_isWorkSession;
                        _timeRemaining = _isWorkSession ? WorkTime : BreakTime;
                    }
                }
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

        private async Task LoadTimeSettings(AccountDTO user)
        {
            if (user == null)
                return;

            var usersSettings = await _settingsService.GetByArgumentAsync(new SettingsDTO { UserId = user.UserId });
            if (usersSettings != null && string.IsNullOrEmpty(usersSettings.Error) && usersSettings.Result != null)
            {
                WorkTime = int.Parse(usersSettings.Result.PomodoroTimer);
                BreakTime = int.Parse(usersSettings.Result.PomodoroBreak);
            }
            _timeRemaining = WorkTime;
        }
    }
}
