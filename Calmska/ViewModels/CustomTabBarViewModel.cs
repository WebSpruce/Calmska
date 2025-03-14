using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    internal partial class CustomTabBarViewModel : ObservableObject
    {
        private static string _selectedTab = "PomodoroPage";
        private static bool _isColorSelectedItem = App.Current.Resources.TryGetValue("Primary01", out var colorSelectedItem);
        [ObservableProperty]
        private string _pomodoroButtonColor;
        [ObservableProperty]
        private string _tipsButtonColor;
        [ObservableProperty]
        private string _settingsButtonColor;

        public CustomTabBarViewModel()
        {
            UpdateButtonColors();
            Shell.Current.Navigated += (s, e) =>
            {
                if (Shell.Current.CurrentPage != null)
                {
                    string? currentPage = Shell.Current.CurrentPage.GetType().Name;
                    UpdateSelectedTab(currentPage);
                }
            };
        }
        [RelayCommand]
        private async void Navigate(string route)
        {
            try
            {
                if (_selectedTab != route)
                {
                    _selectedTab = route;
                    UpdateButtonColors();
                    await Shell.Current.GoToAsync($"{route}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", "Something is wrong.", "Close");
            }
           
        }
        private void UpdateButtonColors()
        {
            PomodoroButtonColor = _selectedTab == "PomodoroPage" ? "#344E41" : "#EA7649";
            TipsButtonColor = _selectedTab == "TipsPage" ? "#344E41" : "#EA7649";
            SettingsButtonColor = _selectedTab == "SettingsPage" ? "#344E41" : "#EA7649";
        }
        private void UpdateSelectedTab(string currentPage)
        {
            _selectedTab = currentPage switch
            {
                "PomodoroPage" => "PomodoroPage",
                "TipsPage" => "TipsPage",
                "SettingsPage" => "SettingsPage",
                _ => _selectedTab
            };
            UpdateButtonColors();
        }
    }
}
