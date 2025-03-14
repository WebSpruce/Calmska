using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Calmska.ViewModels
{
    internal partial class AppShellViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task GoBack() => await Shell.Current.GoToAsync("..");
    }
}
