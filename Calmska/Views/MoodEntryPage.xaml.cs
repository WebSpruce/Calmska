namespace Calmska.Views;

public partial class MoodEntryPage : ContentPage
{
    public MoodEntryPage()
	{
		InitializeComponent();
	}
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var userJson = await SecureStorage.Default.GetAsync("user_info");
        if (string.IsNullOrEmpty(userJson))
        {
            if (Shell.Current.Items.Count > 0)
            {
                foreach (var item in Shell.Current.Items)
                {
                    foreach (var section in item.Items)
                    {
                        foreach (var content in section.Items)
                        {
                            if (content.Route == nameof(LoginPage) || content.Title == nameof(LoginPage))
                            {
                                await Shell.Current.GoToAsync($"..");
                            }
                            else
                            {

                            }
                            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                        }
                    }
                }
            }
            else
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}