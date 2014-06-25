using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Overheads.Helpers
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? isVisible = (bool?)value;

            if (parameter == null)
            {
                return BoolToVisibility(isVisible);
            }
            else
            {
                // do the opposite
                return BoolToVisibility(!isVisible);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }

        public static Visibility BoolToVisibility(bool? convert, bool opposite = false)
        {
            bool? tempconvert = convert;

            if (opposite == true)
            {
                tempconvert = !tempconvert;
            }

            return tempconvert ?? false ? Visibility.Visible : Visibility.Collapsed;
        }
    }   
}
