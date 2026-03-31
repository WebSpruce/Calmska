using Calmska.Services.Interfaces;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;
using Calmska.Application.DTO;
using Calmska.Domain.Common;

namespace Calmska.ViewModels
{
    internal partial class RegisterViewModel : ObservableObject
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
        private string _ePasswordAgain = string.Empty;
        [ObservableProperty]
        private bool _isPasswordMode = true;
        [ObservableProperty]
        private string _passwordBtnIcon = EyeHiddenIcon;
        [ObservableProperty]
        private bool _isActivityIndicatorRunning = false;

        private readonly IAccountService _accountService;
        private CancellationTokenSource _cts;
        public RegisterViewModel(IAccountService accountService)
        {
            _accountService = accountService;
        }
        
        [RelayCommand]
        public void OnAppearing()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
        }
        [RelayCommand]
        public void OnDisappearing()
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
        internal async Task RegisterAsync(CancellationToken token)
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
                if(EPassword != EPasswordAgain)
                {
                    await ShowErrorMessage("The password cannot be empty. Please try again.");
                    return;
                }
                
                var accountFromDb = await _accountService.GetByArgumentAsync(new AccountDTO { Email = EEmail }, linkedCts.Token);
                if(accountFromDb.Result != null)
                {
                    await ShowErrorMessage("The account with this email already exists.");
                    return;
                }
                
                OperationResultT<bool> isSignedUp = await _accountService.AddAsync(
                    new AccountDTO { 
                        Email = EEmail.ToLower(),
                        UserName = EEmail.ToLower(),
                        PasswordHashed = EPassword
                    }, linkedCts.Token);

                if (isSignedUp.Result)
                {
                    await ShowErrorMessage("You created the account! You can sign in.");
                    await GoToLogin();
                }
                else
                {
                    await ShowErrorMessage("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Warning", "Something is wrong. Try again.", "Close");
            }
            finally
            {
                IsActivityIndicatorRunning = false; 
            }
        }
        [RelayCommand]
        internal async Task GoToLogin()
        {
            await MauiControlsApplication.Current.MainPage.Navigation.PopAsync();
        }
        
        private async Task ShowErrorMessage(string message)
        {
#if ANDROID
            await Toast.Make(message, ToastDuration.Short, 14).Show();
#else
            await MauiApplication.Current.MainPage.DisplayAlert("Error", message, "OK");
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
