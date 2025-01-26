using Calmska.Models.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;

namespace Calmska.ViewModels
{
    internal partial class PomodoroViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _helloMessage = string.Empty;
        public PomodoroViewModel()
        {
            string userJson = SecureStorage.Default.GetAsync("user_info").Result ?? string.Empty;
            var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
            HelloMessage = user?.UserName ?? string.Empty;
        }
    }
}
