using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    /// <summary>
    /// Converts between strings and integers.
    /// </summary>
    [ValueConversion(typeof(string), typeof(int))]
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Check if parameter exists. If parameter is true, method returns integers. Otherwise, returns string of value (value should be an integer then)
            object res = null;
            if (parameter == null || (bool)parameter == true)
            {
                if (value.GetType() != typeof(string))
                    throw new InvalidOperationException("Value must be a single character string");

                string val = (string)value;
                if (val.Length > 1)
                    throw new InvalidOperationException("Value must be a single character string");

                // Try to get integer from value.
                int temp = -1;
                if (!Int32.TryParse(val, out temp))
                    throw new InvalidOperationException("Conversion failed.");

                res = temp;
            }
            else
            {
                res = ConvertBack(value, targetType, null, culture);
            }

            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(int))
                throw new InvalidOperationException("Value must be an integer.");

            // "Convert" to string.
            int val = (int)value;
            return "" + val;
        }
    }
}
