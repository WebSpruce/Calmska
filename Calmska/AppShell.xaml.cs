using Calmska.Views;
using Calmska.ViewModels;

namespace Calmska
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(PomodoroPage), typeof(PomodoroPage));
            Routing.RegisterRoute(nameof(TipsPage), typeof(TipsPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(TipsListPage), typeof(TipsListPage));
        }
        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);
            Title.Text = Shell.Current.CurrentPage.Title;
        }
    }
}
