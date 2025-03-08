using System.Globalization;

namespace Calmska.Helper
{
    public class IndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is CollectionView collectionView && value != null)
            {
                var items = collectionView.ItemsSource.Cast<object>().ToList();
                var index = items.IndexOf(value);
                return (index % 4 == 1) || (index % 4 == 2) ? Color.FromArgb("#D4A373") : Color.FromArgb("#538A5E");
            }

            return Color.FromArgb("#538A5E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
