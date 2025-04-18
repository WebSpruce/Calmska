using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;
using System.Text.Json;

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
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private readonly IAccountService _accountService;

        public LoginViewModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

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
        internal async void Login()
        {
            IsActivityIndicatorRunning = true;
            try
            {
                var isEmailValid = new MailAddress(EEmail.ToLower());
                if (string.IsNullOrEmpty(EPassword))
                {
#if ANDROID
                    await Toast.Make("The password cannot be empty. Please try again.", ToastDuration.Short, 14).Show();
#endif
                    return;
                }

                OperationResultT<bool> isLoggedIn = await _accountService.LoginAsync(new AccountDTO
                    { Email = EEmail.ToLower(), PasswordHashed = EPassword });
                if (isLoggedIn.Result)
                {
#if ANDROID
                    await Toast.Make("Logged in.", ToastDuration.Short, 14).Show();
#endif
                    var user = await _accountService.GetByArgumentAsync(new AccountDTO { Email = EEmail.ToLower() });
                    var userJson = JsonSerializer.Serialize(user.Result);
                    await SecureStorage.Default.SetAsync("user_info", userJson);

                    // Check if it has a pending navigation from notification
                    string navigateAfterLogin = Preferences.Default.Get("NavigateAfterLogin", string.Empty);
                    Preferences.Default.Remove("NavigateAfterLogin");

                    if (!string.IsNullOrEmpty(navigateAfterLogin))
                    {
#if ANDROID
                        Android.Util.Log.Debug("NavigationDebug", $"Navigating after login to: {navigateAfterLogin}");
#endif
                        await Shell.Current.GoToAsync($"{navigateAfterLogin}", new Dictionary<string, object>
                        {
                            ["navigationAfterLogin"] = true
                        });
                    }
                    else
                    {
                        await Shell.Current.GoToAsync($"{nameof(PomodoroPage)}");
                    }

                }
                else
                {
#if ANDROID
                    await Toast.Make("Invalid email or password. Please try again.", ToastDuration.Short, 14).Show();
#endif
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", "Something is wrong. Try again.", "Close");
#if ANDROID
                Android.Util.Log.Error("LoginDebug", $"Login error: {ex.Message}");
#endif
            }
            finally
            {
                IsActivityIndicatorRunning = false;
            }
        }
        [RelayCommand]
        internal async Task GoToRegister()
        {
            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}");
        }
    }
}
