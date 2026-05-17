using System;
using System.Globalization;
using System.Windows.Data;

namespace SherioAPP.Converters
{
    public class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return boolValue ? 1.0 : 0.25;

            return 0.25;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double opacity)
                return opacity >= 1.0;

            return false;
        }
    }
}

