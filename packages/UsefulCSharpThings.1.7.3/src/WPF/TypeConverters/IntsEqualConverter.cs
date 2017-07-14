using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    public class IntsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            
            // KFreon: Checks if a pair of integers are equal. Parameter required. 
            if (value.GetType() == typeof(int) && parameter.GetType() == typeof(int))
                return (int)value == (int)parameter;
            else
                throw new InvalidCastException("Requires value and parameter to be of type int.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
