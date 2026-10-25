using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace GFDStudio.GUI.Forms;

public class FlagsEnumEditor : UITypeEditor
{
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
    {
        // Indicate this editor uses a dropdown
        return UITypeEditorEditStyle.DropDown;
    }

    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
    {
        if ( provider?.GetService( typeof( IWindowsFormsEditorService ) ) is IWindowsFormsEditorService editorService )
        {
            var enumType = context.PropertyDescriptor.PropertyType;
            if ( enumType.IsConstructedGenericType )
                enumType = enumType.GetGenericArguments()[0];

            var enumValues = Enum.GetValues( enumType );

            CheckedListBox listBox = new CheckedListBox
            {
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };

            // Populate the list with enum flags
            foreach ( var enumValue in enumValues )
            {
                if ( Convert.ToInt64(enumValue) != 0 ) // Exclude "None" or default values if present
                {
                    listBox.Items.Add( enumValue, value is not null && ( (Enum)value ).HasFlag( (Enum)enumValue ) );
                }
            }

            // Display the dropdown
            editorService.DropDownControl( listBox );

            // Combine the selected flags into a single value
            long result = 0;
            foreach ( object item in listBox.CheckedItems )
            {
                result |= Convert.ToInt64(item);
            }

            if ( result == 0 && value is null )
                return null;
            else
                return Enum.ToObject( enumType, result );
        }

        return value;
    }
}
