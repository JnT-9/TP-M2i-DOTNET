using System.Globalization;
using MauiApp1.Models;

namespace MauiApp1.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    TaskConstants.Status.Todo => Colors.DarkOrange,
                    TaskConstants.Status.InProgress => Colors.Blue,
                    TaskConstants.Status.Done => Colors.Green,
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