using System.Windows.Data;
using System.Globalization;

namespace KinoLunticksApp.Converters
{
    class BooleanToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isOccupied)
            {
                return !isOccupied;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
