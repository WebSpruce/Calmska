using Calmska.Models.DTO;
using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.Json;
using Android.Util;
using Calmska.Helper;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Calmska.ViewModels
{
    internal partial class TipsViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private List<Types_TipsFrontendDTO> _types = new();
        [ObservableProperty]
        private Types_TipsFrontendDTO _selectedType = new();

        private readonly ITypesService<Types_TipsDTO> _typesTipsService;
        private readonly IService<MoodHistoryDTO> _moodHistoryService;
        private readonly IService<MoodDTO> _moodService;
        public TipsViewModel(ITypesService<Types_TipsDTO> typesTipsService, IService<MoodHistoryDTO> moodHistoryService, IService<MoodDTO> moodService)
        {
            _typesTipsService = typesTipsService;
            _moodHistoryService = moodHistoryService;
            _moodService = moodService;

            Task.Run(async () => await LoadTypes());
        }
        private async Task LoadTypes()
        {
            try
            {
                var types = await _typesTipsService.GetAllAsync(null, null);
                if (types != null && string.IsNullOrEmpty(types.Error) && types.Result != null)
                {
                    List<Types_TipsFrontendDTO> typesTemp = new List<Types_TipsFrontendDTO>();
                    foreach (var type in types.Result.Items)
                    {
                        string icon = string.Empty;

                        if (type.TypeId == 1)
                            icon = "\ue30a";
                        if (type.TypeId == 2)
                            icon = "\ue407";
                        if (type.TypeId == 3)
                            icon = "\ue8df";
                        if (type.TypeId == 4)
                            icon = "\ue406";
                        if (type.TypeId == 5)
                            icon = "\ue88a";
                        if (type.TypeId == 6)
                            icon = "\ue8f9";
                        if (type.TypeId == 7)
                            icon = "\ue7ef";
                        if (type.TypeId == 8)
                            icon = "\ue2db";
                        if (type.TypeId == 9)
                            icon = "\ue91d";

                        var type_tip = new Types_TipsFrontendDTO { Type = type.Type, TypeId = type.TypeId, IconName = icon };
                        typesTemp.Add(type_tip);
                    }
                    typesTemp = typesTemp.OrderBy(t => t.Type).ToList();
                    Types = typesTemp;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Loading types of tips error: {ex.Message}\n{ex.InnerException}", "Close");
            }
            
        }
        [RelayCommand]
        private async Task TypeSelected()
        {
            try
            {
                if(SelectedType.TypeId != null && SelectedType.TypeId > 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(TipsListPage)}", new Dictionary<string, object>
                    {
                        { "tipType", SelectedType ?? new Types_TipsFrontendDTO() },
                    });
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Changing page error: {ex.Message}\n{ex.InnerException}", "Close");
            }
        }

        [RelayCommand]
        private async Task Analize()
        {
            try
            {
                string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
                var account = JsonSerializer.Deserialize<AccountDTO>(userJson);
                if (account == null)
                {
#if ANDROID
                    Toast.Make("Couldn't load the user's info.", ToastDuration.Short, 14).Show();
#endif
                    return;
                }
                //save last prompt date to avoid spam
                DateTime lastPrompt = Preferences.Default.Get("LastPrompt", DateTime.MinValue);
                DateTime fiveDaysAgo = DateTime.Today.AddDays(-5);
                if (lastPrompt.Date <= fiveDaysAgo) //at least 5 days from last prompt.
                {
                    var history = await _moodHistoryService.SearchAllByArgumentAsync(new MoodHistoryDTO()
                    {
                        UserId = account.UserId
                    }, null, null);
                    if (history != null && history.Result != null)
                    {
                        var recentMoodHistory = history.Result.Items
                            .Where(m => m.Date.HasValue && m.Date.Value.Date >= DateTime.Today.AddDays(-4))
                            .ToList();
                        if (recentMoodHistory.Count >= 5)
                        {
                            string moods = string.Empty;
                            try
                            {
                                foreach (var row in recentMoodHistory)
                                {
                                    var mood = await _moodService.GetByArgumentAsync(new MoodDTO()
                                    {
                                        MoodId = row.MoodId,
                                    });
                                    if (mood.Result == null)
                                        return;
                                    string comma = (recentMoodHistory.IndexOf(row) == recentMoodHistory.Count - 1) ? "," : "";
                                    moods += $"{mood.Result.MoodName}{comma} ";
                                }
                            }
                            catch (Exception ex)
                            {
                                await Toast.Make("Error while downloading your moods.", ToastDuration.Short, 14).Show();
                                return;
                            }
                            
                            Preferences.Default.Set("LastPrompt", DateTime.Now);
                            string prompt = $"You are an empathetic and insightful AI assistant specializing in mood analysis and well-being. The user has entered their mood for several days in the app. Your tasks are:Analyze the mood data:Identify patterns, trends, or fluctuations in the user's mood over the recorded period.Note any significant changes, recurring themes, or potential triggers (if mentioned).Provide supportive feedback:Summarize your observations in a gentle, encouraging, and non-judgmental tone.Acknowledge positive moments and empathize with any challenges.Offer practical, personalized advice:Suggest specific, actionable steps inspired by the concept of “hygge” (coziness, comfort, connection, simple pleasures).Tailor your advice to the user’s mood patterns and any details they’ve shared.Include ideas for daily rituals, environment adjustments, social connections, or mindful activities that promote a hygge lifestyle. User's moods:{moods}. Don't write additional content just write advices. Write up to 100 words.";
                            string llmresponse = await Prompting.SendPromptRequest(prompt);
                            if(string.IsNullOrEmpty(llmresponse))
                            {
                                await Shell.Current.DisplayAlert("Warning", $"Error while getting mood.", "Close");
                                return;
                            }
                            await Shell.Current.DisplayAlert("Analysis", llmresponse, "Close");
                        }
                        else
                        {
                            await Toast.Make("You don't have moods recorded for the last 5 days.", ToastDuration.Long, 14).Show();
                            return;
                        }
                    }
                    else
                    {
                        await Toast.Make("Getting data error.", ToastDuration.Short, 14).Show();
                        return;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Warning", $"You cannot analize your moods right now, please wait at least 5 days (with 5 moods).", "Close");
                    return;  
                }
                
            }
            catch (Exception ex)
            {
                await Toast.Make("Analizing error.", ToastDuration.Short, 14).Show();
                #if ANDROID
                    Log.Debug("Analizing", $"Analizing error: {ex.Message} - {ex.InnerException}");   
                #endif
            }
        }

        [RelayCommand]
        private async Task HyggeInfo()
        {
            await Shell.Current.DisplayAlert("Hygge",
                "Hygge (pronounced 'hoo-gah') is a Danish and Norwegian concept representing the art of coziness, contentment, and well-being found in life's simple pleasures. It embodies warmth, togetherness, and creating comfortable moments—whether enjoying a book under a blanket, sharing meals with loved ones, or savoring quiet moments of gratitude.",
                "OK");
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            SelectedType = new();
        }
    }
}
