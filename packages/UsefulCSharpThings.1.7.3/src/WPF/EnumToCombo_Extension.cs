using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace UsefulThings.WPF
{
    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(e => new { Value = e, DisplayName = e.ToString() });
        }

        /// <summary>
        /// Get value back out of a casted object value.
        /// Can't get the value normally as Object doesn't have a Value Property, thus must cast back, but has to be done in the same assembly.
        /// </summary>
        /// <param name="value">Anonymous value to convert back.</param>
        /// <returns>Value of anonymous type.</returns>
        public static object GetValueBack(object value)
        {
            var dummy = new { Value = new object(), DisplayName = "" };
            dummy = CastTo(value, dummy);
            return dummy.Value;
        }


        
        static T CastTo<T>(Object value, T TargetType)
        {
            return (T)value;
        }
    }
}
