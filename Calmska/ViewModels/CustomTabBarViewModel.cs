using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    public partial class CustomTabBarViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _selectedTab = "PomodoroPage";
        [ObservableProperty]
        private Color _pomodoroButtonColor;
        [ObservableProperty]
        private Color _tipsButtonColor;
        [ObservableProperty]
        private Color _settingsButtonColor;
        
        private readonly Color _selectedColor = Color.FromArgb("#344E41");
        private readonly Color _defaultColor = Color.FromArgb("#EA7649");

        public CustomTabBarViewModel()
        {
            UpdateButtonColors();
            Shell.Current.Navigated += (s, e) =>
            {
                string currentPageName = Shell.Current.CurrentPage?.GetType().Name;
                if (!string.IsNullOrEmpty(currentPageName) && SelectedTab != currentPageName)
                {
                    SelectedTab = currentPageName;
                }
            };
        }
        [RelayCommand]
        private async void Navigate(string route)
        {
            try
            {
                if (Shell.Current.CurrentPage?.GetType().Name != route)
                {
                    await Shell.Current.GoToAsync($"//{route}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", $"Navigation error: {ex.Message}.", "Close");
            }
           
        }
        partial void OnSelectedTabChanged(string value)
        {
            UpdateButtonColors();
        }
        private void UpdateButtonColors()
        {
            PomodoroButtonColor = SelectedTab == "PomodoroPage" ? _selectedColor : _defaultColor;
            TipsButtonColor = SelectedTab == "TipsPage" ? _selectedColor : _defaultColor;
            SettingsButtonColor = SelectedTab == "SettingsPage" ? _selectedColor : _defaultColor;
        }
    }
}
