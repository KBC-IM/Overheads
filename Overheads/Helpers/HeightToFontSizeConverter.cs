using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Overheads.Helpers
{
    class HeightToFontSizeConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var height = (double)value;
            if (parameter != null)
            {
                string multiplierS = parameter.ToString();
                double multiplier;
                if (double.TryParse(multiplierS, out multiplier))
                    height *= multiplier;
            }

            return .65 * height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo cluture)
        {
            throw new NotImplementedException();
        }
    }
}
