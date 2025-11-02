using System.ComponentModel;
using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.Json;
using Android.Util;
using Calmska.Services;
using Calmska.Views;
using Plugin.Maui.Audio;

namespace Calmska.ViewModels
{
    public partial class PomodoroViewModel : ObservableObject
    {
        private readonly PomodoroTimerService _timerService;
        private readonly IService<SettingsDTO> _settingsService;

        [ObservableProperty]
        private string _navBarTitle = string.Empty;

        public string TimeRemaining => _timerService.FormattedTime;
        public string TimeType => _timerService.TimeType;
        public float ProgressValue => _timerService.ProgressValue;
        public bool IsRunning => _timerService.IsRunning;
        public string PlayPauseIcon => IsRunning ? IconFont.Pause : IconFont.Play_arrow;

        public PomodoroViewModel(PomodoroTimerService timerService, IService<SettingsDTO> settingsService)
        {
            _timerService = timerService;
            _settingsService = settingsService;

            _timerService.PropertyChanged += OnTimerServicePropertyChanged;
        }

        private void OnTimerServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // it notifies the ui when a property changes
            OnPropertyChanged(e.PropertyName);
            if (e.PropertyName == nameof(PomodoroTimerService.IsRunning))
            {
                OnPropertyChanged(nameof(PlayPauseIcon));
            }
        }

        [RelayCommand]
        private void PlayPause()
        {
            if (_timerService.IsRunning)
            {
                _timerService.Pause();
            }
            else
            {
                _timerService.Start();
            }
        }

        [RelayCommand]
        private void ResetTimer()
        {
            _timerService.Reset();
        }

        public async Task OnAppearing()
        {
            try
            {
                string userJson = await SecureStorage.Default.GetAsync("user_info") ?? string.Empty;
                if (string.IsNullOrEmpty(userJson))
                {
                    await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
                    return;
                }
                var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
                NavBarTitle = user?.UserName ?? "User";
                await LoadTimeSettings(user);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Could not load user settings.", "OK");
            }
        }

        private async Task LoadTimeSettings(AccountDTO user)
        {
            if (user == null) return;
            var settings = await _settingsService.GetByArgumentAsync(new SettingsDTO { UserId = user.UserId });
            if (settings?.Result != null)
            {
                int work = int.Parse(settings.Result.PomodoroTimer);
                int breakTime = int.Parse(settings.Result.PomodoroBreak);
                _timerService.UpdateSettings(work, breakTime);
            }
        }

        public void Dispose()
        {
            _timerService.PropertyChanged -= OnTimerServicePropertyChanged;
        }
    }
    
}
