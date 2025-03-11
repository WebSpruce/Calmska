using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;


namespace Calmska.ViewModels
{
    internal partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _lUserName = string.Empty;
        [ObservableProperty]
        private string _eUserName = string.Empty;

        [ObservableProperty]
        private string _eWorkingTimeHours = string.Empty;
        [ObservableProperty]
        private string _eWorkingTimeMinutes = string.Empty;
        [ObservableProperty]
        private string _eWorkingTimeSeconds = string.Empty;
        [ObservableProperty]
        private string _eBreakTimeHours = string.Empty;
        [ObservableProperty]
        private string _eBreakTimeMinutes = string.Empty;
        [ObservableProperty]
        private string _eBreakTimeSeconds = string.Empty;

        private AccountDTO? _accountLogged;


        private readonly IService<SettingsDTO> _settingsService;
        private readonly IAccountService _accountService;
        public SettingsViewModel(IService<SettingsDTO> settingsService, IAccountService accountService)
        {
            _settingsService = settingsService;
            _accountService = accountService;
            string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
            _accountLogged = JsonSerializer.Deserialize<AccountDTO>(userJson);
            if (_accountLogged == null)
            {
#if ANDROID
                    Toast.Make("Couldn't load the user's settings.", ToastDuration.Short, 14).Show();
#endif
                return;
            }
            LUserName = _accountLogged != null ? (_accountLogged.UserName ?? string.Empty) : string.Empty;
            EUserName = _accountLogged != null ? (_accountLogged.UserName ?? string.Empty) : string.Empty;

            LoadSettingsElseCreate(_accountLogged ?? new AccountDTO());
        }

        [RelayCommand]
        private async Task SaveUsername()
        {
            if (_accountLogged == null)
                return;

            AccountDTO accountToUpdate = _accountLogged;
            accountToUpdate.UserName = EUserName;
            accountToUpdate.PasswordHashed = string.Empty;

            OperationResultT<bool> isUpdated = await _accountService.UpdateAsync(accountToUpdate);
            if (isUpdated != null && isUpdated.Result)
            {
#if ANDROID
                await Toast.Make($"Username saved.", ToastDuration.Short, 14).Show();
#endif
                bool isRemoved = SecureStorage.Default.Remove("user_info");
                if (isRemoved)
                {
                    var userJson = JsonSerializer.Serialize(accountToUpdate);
                    await SecureStorage.Default.SetAsync("user_info", userJson);
                    LUserName = EUserName;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning.", "The username is saved, but to see the updated data, please relaunch the app.", "Close");
                }
            }
            else
            {
#if ANDROID
                await Toast.Make($"Couldn't save your username. Please, try again.", ToastDuration.Short, 14).Show();
#endif
            }
        }
        [RelayCommand]
        private async Task SaveSettings()
        {
            if (_accountLogged == null)
                return;
            var usersSettingsToUpdate = await _settingsService.GetByArgumentAsync(new SettingsDTO { UserId = _accountLogged.UserId });
            if (usersSettingsToUpdate != null && string.IsNullOrEmpty(usersSettingsToUpdate.Error) && usersSettingsToUpdate.Result != null)
            {
                int workInSeconds = ConvertToSeconds(int.Parse(EWorkingTimeHours), int.Parse(EWorkingTimeMinutes), int.Parse(EWorkingTimeSeconds));
                int breakInSeconds = ConvertToSeconds(int.Parse(EBreakTimeHours), int.Parse(EBreakTimeMinutes), int.Parse(EBreakTimeSeconds));

                usersSettingsToUpdate.Result.PomodoroBreakFloat = breakInSeconds;
                usersSettingsToUpdate.Result.PomodoroTimerFloat = workInSeconds;
                if (string.IsNullOrEmpty(usersSettingsToUpdate.Result.Color)) { usersSettingsToUpdate.Result.Color = null; }
                var isUpdated = await _settingsService.UpdateAsync(usersSettingsToUpdate.Result);
                if (isUpdated != null && isUpdated.Error == string.Empty && isUpdated.Result)
                {
#if ANDROID
                    await Toast.Make($"Settings saved.", ToastDuration.Short, 14).Show();
#endif
                }
                else
                {
#if ANDROID
                    await Toast.Make($"Couldn't save your settings. Please, try again.", ToastDuration.Short, 14).Show();
#endif
                }
            }
            else
            {
#if ANDROID
                await Toast.Make($"Error while loading settings: {usersSettingsToUpdate.Error}", ToastDuration.Long, 14).Show();
#endif
                return;
            }
        }
        [RelayCommand]
        private async Task Logout()
        {
            bool isRemoved = SecureStorage.Default.Remove("user_info");
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            Shell.Current.Items.Remove(Shell.Current.CurrentItem);
        }
        private async Task LoadSettingsElseCreate(AccountDTO user)
        {
            var usersSettings = await _settingsService.GetByArgumentAsync(new SettingsDTO { UserId = user.UserId });
            if (!string.IsNullOrEmpty(usersSettings.Error))
            {
                if (usersSettings.Error.Contains("NotFound"))
                {
                    SettingsDTO newSettings = new SettingsDTO
                    {
                        UserId = user.UserId,
                        PomodoroTimerFloat = 2700f,
                        PomodoroBreakFloat = 300f
                    };
                    var isAdded = await _settingsService.AddAsync(newSettings);
                    if(isAdded != null && isAdded.Error == string.Empty && isAdded.Result)
                        usersSettings.Result = newSettings;
                    else
                    {
#if ANDROID
                        await Toast.Make($"Error while creating settings {isAdded?.Error}", ToastDuration.Long, 14).Show();
#endif
                    }

                }
                else
                {
#if ANDROID
                    await Toast.Make($"Error while loading settings: {usersSettings.Error}", ToastDuration.Long, 14).Show();
#endif
                    return;
                }
            }
            TimeSpan workingTime = ConvertTime(!string.IsNullOrEmpty(usersSettings.Result?.PomodoroTimer) ? int.Parse(usersSettings.Result.PomodoroTimer) : 0);
            TimeSpan breakTime = ConvertTime(!string.IsNullOrEmpty(usersSettings.Result?.PomodoroBreak) ? int.Parse(usersSettings.Result.PomodoroBreak) : 0);
            EWorkingTimeHours = workingTime.Hours.ToString();
            EWorkingTimeMinutes = workingTime.Minutes.ToString();
            EWorkingTimeSeconds = workingTime.Seconds.ToString();
            EBreakTimeHours = breakTime.Hours.ToString();
            EBreakTimeMinutes = breakTime.Minutes.ToString();
            EBreakTimeSeconds = breakTime.Seconds.ToString();
        }
        private TimeSpan ConvertTime(int seconds)
        {
            int hours = seconds >= 3600 ? seconds / 3600 : 0;
            int minutes = (seconds % 3600) >= 60 ? (seconds % 3600) / 60 : 0;
            int remainingSeconds = (seconds % 60 > 0 && hours == 0 && minutes == 0) ? seconds % 60 : 0;
            return new TimeSpan(hours, minutes, remainingSeconds);
        }
        public static int ConvertToSeconds(int hours, int minutes, int seconds)
        {
            return (hours * 3600) + (minutes * 60) + seconds;
        }
    }
}