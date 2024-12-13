using System.Windows.Data;
using System.Globalization;

namespace KinoLunticksApp.Converters
{
    public class ActorNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string lastName && values[1] is string firstName)
            {
                return $"{lastName} {firstName[0]}.";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
