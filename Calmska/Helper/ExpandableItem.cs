using CommunityToolkit.Mvvm.ComponentModel;

namespace Calmska.Helper
{
    public partial class ExpandableItem<T> : ObservableObject
    {
        [ObservableProperty]   // ✅ make it observable
        private T _data;
        [ObservableProperty]
        private bool _isExpanded;
        [ObservableProperty]
        private object _isDefaultTextVisible = true;
        [ObservableProperty]
        private Color _borderBGColor = Color.FromArgb("#538A5E");
        [ObservableProperty]
        private int _borderHeight = 70;
        partial void OnIsExpandedChanged(bool value)
        {
            BorderHeight = value ? 140 : 70;
            IsDefaultTextVisible = !value;
        }
    }
}
