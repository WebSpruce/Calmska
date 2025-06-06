﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Calmska.Services.Interfaces;
using Calmska.Models.Models;
using Calmska.Models.DTO;
using Calmska.Services.Services;
using System.Text.Json;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using System.Data;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Net.Http.Json;
using Calmska.Helper;
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

        private bool _navigationAfterLogin = false;
        private const int NOTIFICATION_ID = 1002;
        private readonly IService<MoodHistoryDTO> _moodhistoryService;
        private readonly IService<MoodDTO> _moodService;
        private IQueryAttributable _queryAttributableImplementation;

        public MoodEntryPageViewModel(IService<MoodHistoryDTO> moodHistoryService, IService<MoodDTO> moodService)
        {
            _moodhistoryService = moodHistoryService;
            MoodText = string.Empty;
            _moodService = moodService;
            
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
        private async Task SubmitMood()
        {
            try
            {
#if ANDROID
                var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
                notificationManager.Cancel(NOTIFICATION_ID);
#endif
                string prompt = $"User's description of their current mood: \"{MoodText}\". Based on the user's description of their current mood, select the most appropriate mood name from the following list: 'Inspired, Grateful, Curious, Worried, Anxious, Lonely, Bored, Indifferent, Accepting, Resentful, Determined, Peaceful, Overwhelmed, Content, Pensive, Reserved, Reflective, Hopeless, Calm, Confident, Proud, Frustrated, Insecure, Tranquil, Nostalgic, Ecstatic, Cheerful, Energized, Jealous, Irritated, Guilty, Serene, Disappointed, Melancholic, Neutral.' Respond with only one word from this list and nothing else.";
                string llmresponse = await Prompting.SendPromptRequest(prompt);
                if(string.IsNullOrEmpty(llmresponse))
                {
                    await Shell.Current.DisplayAlert("Warning", $"Error while getting mood.", "Close");
                    return;
                }
                var mood = await _moodService.GetByArgumentAsync(new MoodDTO { MoodName = llmresponse, MoodId = null });
                if (mood == null || mood.Result == null || mood.Error != string.Empty)
                {
                    await Shell.Current.DisplayAlert("Warning", $"Error while getting mood.", "Close");
                    return;
                }
                string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
                var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
                if (user == null)
                {
                    await Shell.Current.DisplayAlert("Warning", "Error while getting user's information.", "Close");
                    return;
                }
                var result = await _moodhistoryService.AddAsync(new MoodHistoryDTO { Date = DateTime.Now, MoodId = mood.Result.MoodId, UserId = user.UserId });
                if (result == null || result?.Result == null || result.Error != string.Empty)
                {
                    await Shell.Current.DisplayAlert("Warning", $"Error while getting user's information.\n{result?.Error}", "Close");
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

                if(_navigationAfterLogin)
                    await Shell.Current.GoToAsync(nameof(PomodoroPage));
                else
                    await Shell.Current.GoToAsync("..");
                    
            }
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", "Error while getting user's information.", "Close");
                return;
            }

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            try
            {
                if (query.ContainsKey("navigationAfterLogin"))
                {
                    _navigationAfterLogin = Convert.ToBoolean(query["navigationAfterLogin"]);
                }
            }
            catch (Exception ex)
            {
                Shell.Current.DisplayAlert("Error", "Error while loading data.", "Close");
            }
        }
    }
}
