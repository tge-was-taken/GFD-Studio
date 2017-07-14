using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(parameter == null || value == null)
                return null;
            
            // KFreon: Try to convert the value of an enum to a string.
            Type parameterType = (Type)parameter;
            if (!parameterType.IsEnum)
                return null;

            if (value.GetType() == parameterType)
                return value.ToString();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            
            // KFreon: Try to convert string to enum.
            Type parameterType = (Type)parameter;
            if (value.GetType() == parameterType)
                return Enum.Parse(parameterType, (string)value);

            return null;
        }
    }
}
