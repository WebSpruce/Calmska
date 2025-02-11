using Calmska.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;

namespace Calmska.ViewModels
{
    internal partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string lUserName = string.Empty;
        [ObservableProperty]
        private string eUserName = string.Empty;

        public SettingsViewModel()
        {
            string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
            var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
            LUserName = user != null ? (user.UserName ?? string.Empty) : string.Empty;
            EUserName = user != null ? (user.UserName ?? string.Empty) : string.Empty;
        }
    }
}
