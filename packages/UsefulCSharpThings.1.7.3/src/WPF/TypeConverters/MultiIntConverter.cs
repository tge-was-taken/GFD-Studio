using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    /// <summary>
    /// Checks if multiple integers are equal.
    /// </summary>
    public class MultiIntConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return null;
            
            // Checks type of all arguments.
            foreach (object value in values)
                if (value.GetType() != typeof(int))
                    throw new InvalidOperationException("Values must be integers.");

            // If there's only one element in values that is distinct, contents of values are equal.
            if (values.Count() > 1 && values.Distinct().Count() == 1)
                return true;
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
