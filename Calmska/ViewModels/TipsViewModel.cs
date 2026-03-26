using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using Android.Util;
using Calmska.Application.DTO;
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
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private readonly ITypesService<Types_TipsDTO> _typesTipsService;
        private readonly IService<MoodHistoryDTO> _moodHistoryService;
        private readonly IService<MoodDTO> _moodService;
        private readonly IAiPromptingService _aiPromptingService;
        public TipsViewModel(ITypesService<Types_TipsDTO> typesTipsService, IService<MoodHistoryDTO> moodHistoryService, IService<MoodDTO> moodService, IAiPromptingService aiPromptingService)
        {
            _typesTipsService = typesTipsService;
            _moodHistoryService = moodHistoryService;
            _moodService = moodService;
            _aiPromptingService = aiPromptingService;

            Task.Run(async () => await LoadTypes());
        }
        private async Task LoadTypes()
        {
            try
            {
                var typesResult = await _typesTipsService.GetAllAsync(null, null);
                if (typesResult != null && string.IsNullOrEmpty(typesResult.Error) && typesResult.Result != null)
                {
                    List<Types_TipsFrontendDTO> typesTemp = typesResult.Result.Items.Select(type => MapTypeToFrontendDTO(type)).ToList();
                    typesTemp = typesTemp.OrderBy(t => t.Type).ToList();
                    Types = typesTemp;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Warning", $"Loading types of tips error: {ex.Message}\n{ex.InnerException}", "Close");
            }
            
        }
        [RelayCommand]
        private async Task TypeSelected()
        {
            try
            {
                IsActivityIndicatorRunning = true;
                if (SelectedType.TypeId != null && SelectedType.TypeId > 0)
                {
                    await Shell.Current.GoToAsync($"{nameof(TipsListPage)}", new Dictionary<string, object>
                    {
                        { "tipType", SelectedType ?? new Types_TipsFrontendDTO() },
                    });
                }
            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"Navigation error: {ex.Message}\n{ex.InnerException}");
            }
            finally
            {
                IsActivityIndicatorRunning = false;
            }
        }

        [RelayCommand]
        private async Task Analize(CancellationToken token)
        {
            try
            {
                IsActivityIndicatorRunning = true;
                string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
                var account = JsonSerializer.Deserialize<AccountDTO>(userJson);
                if (account == null)
                {
                    await ShowToastMessage("Couldn't load the user's info.");
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
                                    string comma = (recentMoodHistory.IndexOf(row) == recentMoodHistory.Count - 1)
                                        ? ","
                                        : "";
                                    moods += $"{mood.Result.MoodName}{comma} ";
                                }
                            }
                            catch (Exception ex)
                            {
                                await Toast.Make("Error while downloading your moods.", ToastDuration.Short, 14).Show();
                                return;
                            }

                            Preferences.Default.Set("LastPrompt", DateTime.Now);
                            string llmresponse = await _aiPromptingService.GetPromptResponseAsync(new PromptRequest(moods, true, false), token);
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

                            await Shell.Current.DisplayAlertAsync("Analysis", llmresponse, "Close");
                        }
                        else
                        {
                            await Toast.Make("You don't have moods recorded for the last 5 days.", ToastDuration.Long,
                                14).Show();
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
                    await Shell.Current.DisplayAlertAsync("Warning",
                        $"You cannot analize your moods right now, please wait at least 5 days (with 5 moods).",
                        "Close");
                    return;
                }

            }
            catch (Exception ex)
            {
                await ShowErrorMessage($"Analysis error: {ex.Message}\n{ex.InnerException}");
#if ANDROID
                Log.Debug("Analizing", $"Analizing error: {ex.Message} - {ex.InnerException}");
#endif
            }
            finally
            {
                IsActivityIndicatorRunning = false;
            }
        }

        [RelayCommand]
        private async Task HyggeInfo()
        {
            await Shell.Current.DisplayAlertAsync("Hygge",
                "Hygge (pronounced 'hoo-gah') is a Danish and Norwegian concept representing the art of coziness, contentment, and well-being found in life's simple pleasures. It embodies warmth, togetherness, and creating comfortable moments—whether enjoying a book under a blanket, sharing meals with loved ones, or savoring quiet moments of gratitude.",
                "OK");
        }
        
        private Types_TipsFrontendDTO MapTypeToFrontendDTO(Types_TipsDTO type)
        {
            string icon = GetIconForTypeId(type.TypeId ?? 0);
            return new Types_TipsFrontendDTO { Type = type.Type, TypeId = type.TypeId, IconName = icon };
        }
        
        private string GetIconForTypeId(int typeId)
        {
            Dictionary<int, string> typeIdToIconMap = new Dictionary<int, string>()
            {
                { 1, "\ue30a" },
                { 2, "\ue407" },
                { 3, "\ue8df" },
                { 4, "\ue406" },
                { 5, "\ue88a" },
                { 6, "\ue8f9" },
                { 7, "\ue7ef" },
                { 8, "\ue2db" },
                { 9, "\ue91d" }
            };

            if (typeIdToIconMap.ContainsKey(typeId))
            {
                return typeIdToIconMap[typeId];
            }
            return string.Empty;
        }
        
        private async Task ShowToastMessage(string message)
        {
#if ANDROID
            await Toast.Make(message, ToastDuration.Short, 14).Show();
#else
        await Shell.Current.DisplayAlertAsync("Warning", message, "OK");
#endif
        }

        private async Task ShowErrorMessage(string message)
        {
            await Shell.Current.DisplayAlertAsync("Warning", message, "Close");
        }

        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            SelectedType = new();
        }
    }
}
