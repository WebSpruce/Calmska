using Calmska.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Calmska.Controls
{
    internal class CircularProgressDrawable : IDrawable
    {
        private readonly PomodoroViewModel _viewModel;

        internal CircularProgressDrawable(PomodoroViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float strokeWidth = 15;
            float radius = 100; // Half of WidthRequest/HeightRequest of GraphicsView
            float centerX = dirtyRect.Width / 2;
            float centerY = dirtyRect.Height / 2;
            float startAngle = 0;
            float sweepAngle = 360 * _viewModel.ProgressValue;

            canvas.StrokeSize = strokeWidth;
            canvas.StrokeColor = Colors.LightGray;
            canvas.DrawCircle(centerX, centerY, radius);
            canvas.Rotate(-90);
            // Draw the red progress
            canvas.StrokeColor = Color.FromArgb("#EA7649");
            canvas.DrawArc(
                centerX - radius - 200, centerY - radius, radius * 2, radius * 2,
                startAngle, sweepAngle, false, false);
        }
    }
}
