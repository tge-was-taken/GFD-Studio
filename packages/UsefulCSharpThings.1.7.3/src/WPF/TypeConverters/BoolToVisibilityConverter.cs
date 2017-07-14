using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    [ValueConversion(typeof(bool?), typeof(Visibility), ParameterType=typeof(bool))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            bool? val = null;

            // KFreon: Cast from correct type. I'm sure there's a better way to do this, but I can't figure it out.
            if (value == null)
                val = (bool?)null;
            else
            {
                Type type = value.GetType();
                if (type == typeof(bool))
                    val = (bool)value;
                else if (type == typeof(bool?))
                    val = (bool?)value;
                else
                    throw new InvalidCastException("Value and parameter must be of type bool or bool?");
            }
            

            // KFreon: Invert if asked to (can't invert null)
            if (parameter != null && (bool)parameter)
                val = !val;


            // KFreon: Convert to Visibility
            if (val == true)
                visibility = Visibility.Visible;
            else if (val == null)
                visibility = Visibility.Hidden;
            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
