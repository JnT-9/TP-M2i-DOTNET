using System.Globalization;
using MauiApp1.Models;

namespace MauiApp1.Converters
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string priority)
            {
                return priority switch
                {
                    TaskConstants.Priority.Low => Colors.Green,
                    TaskConstants.Priority.Medium => Colors.Orange,
                    TaskConstants.Priority.High => Colors.Red,
                    _ => Colors.Gray
                };
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 