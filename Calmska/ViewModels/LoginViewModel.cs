using Calmska.Services.Interfaces;
using Calmska.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;
using System.Text.Json;
using Calmska.Application.DTO;

namespace Calmska.ViewModels
{
    internal partial class LoginViewModel : ObservableObject
    {
        public const string DefaultBackgroundImage = "leaf.png";
        public const string EyeHiddenIcon = "eye_hidden.svg";
        public const string EyeOpenIcon = "eye_open.svg";
        
        [ObservableProperty]
        private string _backgroundImageSource = DefaultBackgroundImage;
        [ObservableProperty]
        private string _eEmail = string.Empty;
        [ObservableProperty]
        private string _ePassword = string.Empty;
        [ObservableProperty]
        private bool _isPasswordMode = true;
        [ObservableProperty]
        private string _passwordBtnIcon = EyeHiddenIcon;
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private readonly IAccountService _accountService;
        private CancellationTokenSource _cts;

        public LoginViewModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [RelayCommand]
        public async Task AppearingAsync()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            
            try
            {
                string? userJson = await SecureStorage.Default.GetAsync("user_info");
                if (string.IsNullOrEmpty(userJson))
                {
                    return;
                }
        
                var user = JsonSerializer.Deserialize<AccountDTO>(userJson ?? string.Empty);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.Email))
                    {
                        await Shell.Current.GoToAsync($"{nameof(PomodoroPage)}");
                        Shell.Current.Items.Remove(Shell.Current.CurrentItem);
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", 
                    "Error checking authentication status.", "OK");
            }
        }
        [RelayCommand]
        public void Disappearing()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        [RelayCommand]
        internal void ChangePasswordVisibility()
        {
            IsPasswordMode = !IsPasswordMode;
            if (IsPasswordMode)
                PasswordBtnIcon = EyeOpenIcon;
            else
                PasswordBtnIcon = EyeHiddenIcon;
        }

        [RelayCommand]
        internal async Task Login(CancellationToken token)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _cts?.Token ?? CancellationToken.None);
            IsActivityIndicatorRunning = true;
            try
            {
                if (!IsValidEmail(EEmail.ToLower()))
                {
                    await ShowErrorMessage("Invalid email format. Please check your input.");
                    return;
                }
                if (string.IsNullOrEmpty(EPassword))
                {
                    await ShowErrorMessage("The password cannot be empty. Please try again.");
                    return;
                }

                var isLoggedIn = await _accountService.LoginAsync(
                    new AccountDTO
                    {
                        Email = EEmail.ToLower(), 
                        PasswordHashed = EPassword
                    }, linkedCts.Token);
                
                if (isLoggedIn.Result)
                {
                    var user = await _accountService.GetByArgumentAsync(new AccountDTO { Email = EEmail.ToLower() }, linkedCts.Token);
                    var userJson = JsonSerializer.Serialize(user.Result);
                    await SecureStorage.Default.SetAsync("user_info", userJson);

                    // Check if it has a pending navigation from notification
                    MauiControlsApplication.Current.MainPage = new AppShell();
                    
                    string navigateAfterLogin = Preferences.Default.Get("NavigateAfterLogin", string.Empty);
                    Preferences.Default.Remove("NavigateAfterLogin");

                    if (!string.IsNullOrEmpty(navigateAfterLogin))
                    {
                        await Task.Delay(100); 
                        await Shell.Current.GoToAsync($"//{navigateAfterLogin}");
                    }
#if ANDROID
                    await Toast.Make("Logged in.", ToastDuration.Short, 14).Show();
#endif
                }
                else
                {
                    await ShowErrorMessage($"Invalid email or password: {isLoggedIn.Error}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Login error: {ex.Message}"); 
                await ShowErrorMessage("An unexpected error occurred. Please try again later.");
            }
            finally
            {
                IsActivityIndicatorRunning = false;
            }
        }
        [RelayCommand]
        internal async Task GoToRegister()
        {
            try
            {
                var registerPage = MauiProgram.Services.GetRequiredService<RegisterPage>();
                await MauiControlsApplication.Current.MainPage.Navigation.PushAsync(registerPage);
            }
            catch (Exception ex)
            {
                await MauiControlsApplication.Current.MainPage.DisplayAlert("Error", "Could not open the registration page.", "OK");
            }
        }
        
        private async Task ShowErrorMessage(string message)
        {
#if ANDROID
            await Toast.Make(message, ToastDuration.Short, 14).Show();
#else
            await MauiControlsApplication.Current.MainPage.DisplayAlert("Error", message, "OK");
#endif
        }
        
        private bool IsValidEmail(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
