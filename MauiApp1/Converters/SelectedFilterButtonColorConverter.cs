using System.Globalization;

namespace MauiApp1.Converters
{
    public class SelectedFilterButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedStatus = value as string ?? string.Empty;
            string buttonStatus = parameter as string ?? string.Empty;

            // If this button's status matches the selected status, highlight it
            if (selectedStatus == buttonStatus)
            {
                return Colors.Blue; // Bright blue for selected button
            }

            // Different colors for different statuses when not selected
            return buttonStatus switch
            {
                "Todo" => Colors.DarkOrange.WithAlpha(0.6f),
                "InProgress" => Colors.Blue.WithAlpha(0.6f),
                "Done" => Colors.Green.WithAlpha(0.6f),
                _ => Colors.Gray.WithAlpha(0.6f), // Gray for "All" button
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 