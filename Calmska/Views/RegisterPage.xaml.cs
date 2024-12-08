using Calmska.ViewModels;

namespace Calmska.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
		BindingContext = new RegisterViewModel();
	}
}