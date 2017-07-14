using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UsefulThings.WPF.TypeConverters
{
    // Mostly got it from here: http://stackoverflow.com/questions/3985876/wpf-binding-a-listbox-to-an-enum-displaying-the-description-attribute
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum theEnum = (Enum)value;
            string description = UsefulThings.General.GetEnumDescription(theEnum);
            return description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
