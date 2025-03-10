using CommunityToolkit.Mvvm.ComponentModel;

namespace Calmska.Helper
{
    internal partial class ExpandableItem<T> : ObservableObject
    {
        public T Data { get; set; }
        [ObservableProperty]
        private bool _isExpanded;
        [ObservableProperty]
        private object _isDefaultTextVisible;
        [ObservableProperty]
        private object _borderBGColor = Color.FromArgb("#538A5E");
        [ObservableProperty]
        private int _borderHeight = 70;
        partial void OnIsExpandedChanged(bool value)
        {
            BorderHeight = value ? 140 : 70;
        }
    }
}
