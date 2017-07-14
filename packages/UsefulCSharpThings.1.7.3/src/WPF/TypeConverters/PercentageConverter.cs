using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic number = value;
            return (number * 100d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            dynamic number = value;
            return (number / 100d);
        }
    }
}
