using Calmska.ViewModels;

namespace Calmska.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();

        SettingsViewModel._instance.OnAppearing();
    }
}