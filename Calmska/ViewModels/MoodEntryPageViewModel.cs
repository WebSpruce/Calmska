using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Calmska.Services.Interfaces;
using Calmska.Models.DTO;
using System.Text.Json;
using Calmska.Models.Models;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using Calmska.Views;

#if ANDROID
using Android.App;
using Android.Content;
using Android.Util;
#endif
namespace Calmska.ViewModels
{
    public partial class MoodEntryPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string _moodText;
        
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private bool _navigationAfterLogin = false;
        private const int NOTIFICATION_ID = 1002;
        private readonly IService<MoodHistoryDTO> _moodhistoryService;
        private readonly IService<MoodDTO> _moodService;
        private readonly IAiPromptingService _aiPromptingService;
        private IQueryAttributable _queryAttributableImplementation;

        public MoodEntryPageViewModel(IService<MoodHistoryDTO> moodHistoryService, IService<MoodDTO> moodService, IAiPromptingService aiPromptingService)
        {
            _moodhistoryService = moodHistoryService;
            MoodText = string.Empty;
            _moodService = moodService;
            _aiPromptingService = aiPromptingService;

#if ANDROID
            Log.Debug("MoodEntryPageViewModel", "Constructor");
#endif
        }

        [RelayCommand]
        private async Task Close()
        {
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        private async Task SubmitMood(CancellationToken token)
        {
            try
            {
                IsActivityIndicatorRunning = true;
#if ANDROID
                var notificationManager =
                    (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
                notificationManager.Cancel(NOTIFICATION_ID);
#endif
                string llmresponse = await _aiPromptingService.GetPromptResponseAsync(new PromptRequest(MoodText, false, true), token);
                if (string.IsNullOrEmpty(llmresponse))
                {
                    await Shell.Current.DisplayAlertAsync("Warning", $"Error while getting mood.", "Close");
                    return;
                }
                if (llmresponse.Contains("Request blocked"))
                {
                    await Shell.Current.DisplayAlertAsync("STOP!", $"{llmresponse}", "Close");
                    return;
                }

                var mood = await _moodService.GetByArgumentAsync(new MoodDTO { MoodName = llmresponse, MoodId = null });
                if (mood == null || mood.Result == null || mood.Error != string.Empty)
                {
                    await Shell.Current.DisplayAlertAsync("Warning", $"Error while getting mood.", "Close");
                    return;
                }

                string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
                var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
                if (user == null)
                {
                    await Shell.Current.DisplayAlertAsync("Warning", "Error while getting user's information.",
                        "Close");
                    return;
                }

                var result = await _moodhistoryService.AddAsync(new MoodHistoryDTO
                {
                    Date = DateTime.Now, MoodId = mood.Result.MoodId, UserId = user.UserId,
                    MoodHistoryId = Guid.NewGuid()
                });
                if (result == null || result?.Result == null || result.Error != string.Empty)
                {
                    await Shell.Current.DisplayAlertAsync("Warning",
                        $"Error while getting user's information.\n{result?.Error}", "Close");
                    return;
                }

                bool isAdded = result.Result;

                if (isAdded)
                {
#if ANDROID
                    await Toast.Make("Success! See you soon!", ToastDuration.Short, 14).Show();
#endif
                }
                else
                {
#if ANDROID
                    await Toast.Make("Something was wrong during the operation.", ToastDuration.Short, 14).Show();
#endif
                }

                if (_navigationAfterLogin)
                    await Shell.Current.GoToAsync(nameof(PomodoroPage));
                else
                    await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", "Error while getting user's information.", "Close");
                return;
            }
            finally
            {
                IsActivityIndicatorRunning = false;
            }

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                if (query.TryGetValue("navigationAfterLogin", out var value))
                    _navigationAfterLogin = Convert.ToBoolean(value);
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlertAsync("Error", "Error while loading data.", "Close");
            }
        }
    }
}
