using Calmska.ViewModels;
using Calmska.Views;

namespace Calmska
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string userJson = SecureStorage.Default.GetAsync("user_info").Result;

            if (string.IsNullOrEmpty(userJson))
            {
                var loginPage = MauiProgram.Services.GetRequiredService<LoginPage>();
                MainPage = new NavigationPage(loginPage);
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}
