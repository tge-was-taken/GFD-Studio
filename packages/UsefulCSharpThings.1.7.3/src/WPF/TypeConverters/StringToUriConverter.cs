using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    /// <summary>
    /// Converts string to URI.
    /// </summary>
    public class StringToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
                
            if (value.GetType() != typeof(string))
                throw new InvalidCastException("Value must be a string.");

            // For now, only allow this if string points to a file on disk.
            if (!File.Exists((string)value))
                return null;

            return new Uri((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
