using System.Globalization;

namespace MauiApp1.Converters
{
    public class EmptyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && string.IsNullOrEmpty(stringValue))
            {
                return parameter?.ToString() ?? "All";
            }
            
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value?.ToString() == (parameter?.ToString() ?? "All"))
            {
                return string.Empty;
            }
            
            return value;
        }
    }
} 