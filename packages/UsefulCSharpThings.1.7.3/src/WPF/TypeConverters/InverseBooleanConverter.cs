using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InvertBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InvertBool(value);
        }

        bool InvertBool(object value)
        {
            // In the case of nullable bool
            if (value == null)
                return false;

            // KFreon: Returns inverse of given boolean
            dynamic boolean = value;
            return !boolean;  // This will fail if not a boolean.
        }
    }
}
