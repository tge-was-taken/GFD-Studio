using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    /// <summary>
    /// Used for when two checkboxes are mutually exclusive, but can both be false.
    /// So, when one goes true, the other goes false, but if both go false, nothing changes.
    /// </summary>
    public class MutuallyExclusiveCheckersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            // Checks type/s
            if (value.GetType() == typeof(bool))
            {
                bool val = (bool)value;
                if (parameter != null && parameter.GetType() == typeof(bool))
                {
                    return (bool)parameter ? !val : val;
                }
                else
                {
                    return val;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Updates code value (not binding)
            if (value.GetType() == typeof(bool))
            {
                bool val = (bool)value;
                if (parameter != null && parameter.GetType() == typeof(bool))
                    return (bool)parameter ? (!val ? (bool?)null : false) : (val ? true : (bool?)null);
                else
                    return val ? true : (bool?)null;
            }
            return false;
        }
    }
}
