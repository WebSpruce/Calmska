using Calmska.Controls;
using Calmska.ViewModels;

namespace Calmska.Views;

public partial class PomodoroPage : ContentPage
{
    private readonly PomodoroViewModel _viewModel;
    public PomodoroPage(PomodoroViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;

        CircularProgress.Drawable = new CircularProgressDrawable(_viewModel);

        // Update UI when ViewModel properties change
        _viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(PomodoroViewModel.ProgressValue))
            {
                CircularProgress.Invalidate();
            }
        };
    }
}