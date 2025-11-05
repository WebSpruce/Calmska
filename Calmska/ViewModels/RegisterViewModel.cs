using Calmska.Models.DTO;
using Calmska.Models.Models;
using Calmska.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;

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
        private string _ePasswordAgain = string.Empty;
        [ObservableProperty]
        private bool _isPasswordMode = true;
        [ObservableProperty]
        private string _passwordBtnIcon = "eye_hidden.svg";
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private readonly IAccountService _accountService;
        public RegisterViewModel(IAccountService accountService)
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
        internal async void Register()
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
                if(EPassword != EPasswordAgain)
                {
#if ANDROID
                    await Toast.Make("The passwords are not the same. Please try again.", ToastDuration.Short, 14).Show();
#endif
                    return;
                }
                var accountFromDb = await _accountService.GetByArgumentAsync(new AccountDTO { Email = EEmail });
                if(accountFromDb.Result != null)
                {
#if ANDROID
                    await Toast.Make("The account with this email already exists.", ToastDuration.Short, 14).Show();
#endif
                    return;
                }
                OperationResultT<bool> isSignedUp = await _accountService.AddAsync(
                    new AccountDTO { 
                        Email = EEmail.ToLower(),
                        UserName = EEmail.ToLower(),
                        PasswordHashed = EPassword
                    });

                if (isSignedUp.Result)
                {
#if ANDROID
                    await Toast.Make("You created the account! You can sign in.", ToastDuration.Short, 14).Show();
#endif
                    await GoToLogin();
                }
                else
                {
#if ANDROID
                    await Toast.Make("Invalid email or password.", ToastDuration.Short, 14).Show();
#endif
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Warning", "Something is wrong. Try again.", "Close");
            }
            finally
            {
                IsActivityIndicatorRunning = false; 
            }
        }
        [RelayCommand]
        internal async Task GoToLogin()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
