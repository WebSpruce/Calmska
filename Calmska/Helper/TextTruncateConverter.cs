using System.Globalization;

namespace Calmska.Helper
{
    public class TextTruncateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text && parameter is string lengthStr && int.TryParse(lengthStr, out int length))
            {
                if (text.Length <= length)
                    return text;

                return text.Substring(0, length) + "...";
            }
            return value ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
