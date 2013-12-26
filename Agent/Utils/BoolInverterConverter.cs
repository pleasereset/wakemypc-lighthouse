using System;
using System.Windows.Data;

namespace ree7.WakeMyPC.Agent.Utils.Converters
{
    public class BoolInverterConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = (bool)value;
            return !b;
        }
    }
}
