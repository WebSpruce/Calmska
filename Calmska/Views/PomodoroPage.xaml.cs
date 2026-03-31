using Calmska.Controls;
using Calmska.ViewModels;

namespace Calmska.Views;

public partial class PomodoroPage : ContentPage
{
    public PomodoroPage(PomodoroViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;

        CircularProgress.Drawable = new CircularProgressDrawable(viewModel);

        // Update UI when ViewModel properties change
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(PomodoroViewModel.ProgressValue))
            {
                CircularProgress.Invalidate();
            }
        };
    }
}