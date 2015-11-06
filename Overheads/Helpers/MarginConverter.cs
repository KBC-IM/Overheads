using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Overheads.Helpers
{
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)value;
            if (parameter != null)
            {
                string multiplierS = parameter.ToString();
                double multiplier;
                if (double.TryParse(multiplierS, out multiplier))
                    height *= multiplier;
            }

            double marginRight = .255 * height;

            return new Thickness(0, 0, marginRight, 0);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
