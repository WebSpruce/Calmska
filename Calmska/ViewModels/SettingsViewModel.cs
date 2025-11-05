using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui;
using System.Text.Json;
using System.Diagnostics;
using Application = Microsoft.Maui.Controls.Application;
using Debug = System.Diagnostics.Debug;

#if ANDROID
using Android.Content;
using Android.App;
using Android.OS;
using Calmska.Platforms.Android.ForegroundServices;
using Calmska.Platforms.Android.BroadcastReceivers;
using Calmska.Platforms.Android;
#endif

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
        private bool _notificationsEnabled = true;
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (_notificationsEnabled != value)
                {
                    _notificationsEnabled = value;
                    Preferences.Default.Set("IsNotificationEnabled", value);
                    int hour = Preferences.Default.Get("NotificationHour", SelectedNotificationTime.Hours);
                    int minute = Preferences.Default.Get("NotificationMinute", SelectedNotificationTime.Minutes);
#if ANDROID
                    UpdateNotification();
#endif
                    OnPropertyChanged();
                }
            }
        }
        private TimeSpan _selectedNotificationTime = new TimeSpan(8, 0, 0);
        public TimeSpan SelectedNotificationTime
        {
            get => _selectedNotificationTime;
            set
            {
                if (_selectedNotificationTime != value)
                {
                    _selectedNotificationTime = value;
                    Preferences.Default.Set("NotificationHour", value.Hours);
                    Preferences.Default.Set("NotificationMinute", value.Minutes);

#if ANDROID
                    UpdateNotification();
#endif
                    OnPropertyChanged();
                }
            }
        }

        private AccountDTO? _accountLogged;
        private const string NotificationServiceKey = "MoodNotificationsEnabled";
        internal static SettingsViewModel _instance;

        private readonly IService<SettingsDTO> _settingsService;
        private readonly IAccountService _accountService;
        public SettingsViewModel(IService<SettingsDTO> settingsService, IAccountService accountService)
        {
            _instance = this;
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

            //NotificationsEnabled = Preferences.Get("NotificationsEnabled", false);
            //var timeString = Preferences.Get("NotificationTime", "08:00:00");
            //SelectedNotificationTime = TimeSpan.Parse(timeString);
        }
        internal void OnAppearing()
        {
            bool isEnabled = Preferences.Default.Get("IsNotificationEnabled", false);
            int hour = Preferences.Default.Get("NotificationHour", 8);
            int minute = Preferences.Default.Get("NotificationMinute", 0);

            NotificationsEnabled = isEnabled;
            SelectedNotificationTime = new TimeSpan(hour, minute, 0);
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
        private void Logout()
        {
            try
            {
                SecureStorage.Default.Remove("user_info");
                Preferences.Default.Clear(); 
                var loginPage = MauiProgram.Services.GetRequiredService<LoginPage>();
                Application.Current.MainPage = new NavigationPage(loginPage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Logout error: {ex.Message}");
                Application.Current.MainPage.DisplayAlert("Error", "Logout failed. Please try again.", "OK");
            }
        }
#if ANDROID
        private void UpdateNotification()
        {
            var context = Android.App.Application.Context;

            if (NotificationsEnabled)
            {
                var intent = new Intent(context, typeof(MoodNotificationService));
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    context.StartForegroundService(intent);
                else
                    context.StartService(intent);
            }
            else
            {
                NotificationScheduler.CancelNotification(context);
            }
        }
#endif
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