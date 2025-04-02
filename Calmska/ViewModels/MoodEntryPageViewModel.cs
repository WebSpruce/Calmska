using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
#if ANDROID
using Android.App;
using Android.Content;
#endif
namespace Calmska.ViewModels
{
    public partial class MoodEntryPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _moodText;


        private const int NOTIFICATION_ID = 1002;
        public MoodEntryPageViewModel()
        {
            MoodText = string.Empty;
        }

        [RelayCommand]
        private async Task SubmitMood()
        {
#if ANDROID
            var notificationManager = (NotificationManager)Android.App.Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.Cancel(NOTIFICATION_ID);
#endif

            await Shell.Current.GoToAsync("..");
        }
    }
}
