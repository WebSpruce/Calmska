using Calmska.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    internal partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _backgroundImageSource = "leaf.png";
        [ObservableProperty]
        private string _eEmail = string.Empty;
        [ObservableProperty]
        private string _ePassword = string.Empty;
        [ObservableProperty]
        private bool _isPasswordMode = true;
        [ObservableProperty]
        private string _passwordBtnIcon = "eye_hidden.svg";
        [RelayCommand]
        internal async Task GoToLogin()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }
    }
}
