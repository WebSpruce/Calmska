using Calmska.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
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
        internal void ChangePasswordVisibility()
        {
            IsPasswordMode = !IsPasswordMode;
            if (IsPasswordMode)
                PasswordBtnIcon = "eye_open.svg";
            else
                PasswordBtnIcon = "eye_hidden.svg";
        }
        [RelayCommand]
        internal void Login()
        {

        }
        [RelayCommand]
        internal async Task GoToRegister()
        {
            await Shell.Current.GoToAsync($"..");
        }
    }
}
