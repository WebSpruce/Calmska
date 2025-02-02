using Calmska.ViewModels;

namespace Calmska.Controls;

public partial class CustomTabBar : ContentView
{
    public CustomTabBar()
	{
		InitializeComponent();
		BindingContext = new CustomTabBarViewModel();
	}
}