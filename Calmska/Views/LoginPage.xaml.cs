using Calmska.Models.DTO;
using System.Text.Json;

namespace Calmska.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        string? userJson = SecureStorage.Default.GetAsync("user_info").Result;
        if (!string.IsNullOrEmpty(userJson))
        {
            var user = JsonSerializer.Deserialize<AccountDTO>(userJson);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                await Shell.Current.GoToAsync($"{nameof(PomodoroPage)}");
                Shell.Current.Items.Remove(Shell.Current.CurrentItem);
            }
        }
    }
}