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
            await Shell.Current.DisplayAlert("Error", 
                "Error checking authentication status.", "OK");
        }
    }
}