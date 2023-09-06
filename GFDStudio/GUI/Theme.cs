using GFDLibrary.Rendering.OpenGL;
using GFDStudio.GUI.DataViewNodes;
using MetroSet_UI.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace GFDStudio.GUI.Forms
{
    public class Theme
    {
        public static Color DarkBG { get; set; } = Color.FromArgb( 30, 30, 30 );
        public static Color DarkText { get; set; } = Color.FromArgb( 220, 220, 220 );
        public static Color LightBG { get; set; } = Color.FromArgb( 240, 240, 240 );
        public static Color LightText { get; set; } = Color.FromArgb( 20, 20, 20 );

        public static void Apply(MetroSetForm form)
        {
            var style = MetroSet_UI.Enums.Style.Dark;
            if ( MainForm.settings.DarkMode )
            {
                if (form.MainMenuStrip != null)
                    form.MainMenuStrip.Renderer = new DarkMenuRenderer();
            }
            else
            {
                style = MetroSet_UI.Enums.Style.Light;
                if ( form.MainMenuStrip != null )
                    form.MainMenuStrip.Renderer = new ToolStripProfessionalRenderer();
            }
            GUI.Controls.ModelViewControl.Instance.Refresh();

            form.Style = style;

            foreach ( var control in EnumerateControls( form ) )
            {
                dynamic ctrl = control as dynamic;
                if ( PropertyExists( ctrl, "Style" ) )
                    ctrl.Style = style;

                if ( MainForm.settings.DarkMode )
                {
                    ctrl.BackColor = DarkBG;
                    ctrl.ForeColor = DarkText;
                }
                else
                {
                    ctrl.BackColor = LightBG;
                    ctrl.ForeColor = LightText;
                    if ( PropertyExists( ctrl, "BackgroundColor" ) )
                        ctrl.BackgroundColor = LightBG;
                }

                if ( ctrl.GetType() == typeof( MenuStrip ) )
                    RecursivelySetColor(ctrl);
            }

        }

        public static void RecursivelySetColor( dynamic ctrl )
        {
            if ( ctrl.GetType() == typeof( MenuStrip ) )
                foreach ( dynamic item in ctrl.Items )
                {
                    if ( MainForm.settings.DarkMode )
                        item.ForeColor = DarkText;
                    else
                        item.ForeColor = LightText;

                    RecursivelySetColor( item );
                }
            else if ( ctrl.GetType() == typeof( ContextMenuStrip ) )
            {
                if ( MainForm.settings.DarkMode )
                    ctrl.Renderer = new DarkMenuRenderer();
                else
                    ctrl.Renderer = new ToolStripProfessionalRenderer();

                if ( MainForm.settings.DarkMode )
                    ctrl.ForeColor = DarkText;
                else
                    ctrl.ForeColor = LightText;

                foreach(var item in ctrl.Items)
                    RecursivelySetColor( item );
            }
            else if ( ctrl.GetType() == typeof( ToolStripMenuItem ) || ctrl.GetType() == typeof( ToolStripItem ) )
                foreach ( dynamic item in ctrl.DropDownItems )
                {
                    if ( MainForm.settings.DarkMode )
                        item.ForeColor = DarkText;
                    else
                        item.ForeColor = LightText;

                    RecursivelySetColor( item );
                }
        }

        public static bool PropertyExists( dynamic obj, string name )
        {
            if ( obj == null ) return false;
            if ( obj is IDictionary<string, object> dict )
            {
                return dict.ContainsKey( name );
            }
            return obj.GetType().GetProperty( name ) != null;
        }

        public static List<Control> EnumerateControls( Form form )
        {
            List<Control> controls = new List<Control>();
            foreach ( Control control in form.Controls )
            {
                controls.Add( control );
                if ( control.Controls != null )
                    controls.AddRange( EnumerateChildren( control ) );
            }
            return controls;
        }

        public static List<Control> EnumerateChildren( Control root )
        {
            List<Control> controls = new List<Control>();
            foreach ( Control control in root.Controls )
            {
                controls.Add( control );
                if ( control.Controls != null )
                    controls.AddRange( EnumerateChildren( control ) );
            }
            return controls;
        }


        public class DarkMenuRenderer : ToolStripProfessionalRenderer
        {
            public DarkMenuRenderer() : base( new CustomColors() ) { }
        }
        public class CustomColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return DarkBG; }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return DarkBG; }
            }
            public override Color SeparatorDark
            {
                get { return DarkBG; }
            }
            public override Color SeparatorLight
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color MenuStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color MenuStripGradientEnd
            {
                get { return DarkBG; }
            }

            public override Color ButtonCheckedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ButtonCheckedHighlight
            {
                get { return DarkBG; }
            }

            public override Color ButtonPressedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ButtonPressedHighlight
            {
                get { return DarkBG; }
            }

            public override Color ButtonSelectedGradientMiddle
            {
                get { return DarkBG; }
            }

            public override Color ButtonSelectedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ButtonSelectedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ButtonSelectedHighlight
            {
                get { return DarkBG; }
            }

            public override Color OverflowButtonGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color OverflowButtonGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color OverflowButtonGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color CheckBackground
            {
                get { return DarkBG; }
            }
            public override Color CheckPressedBackground
            {
                get { return DarkBG; }
            }
            public override Color CheckSelectedBackground
            {
                get { return DarkBG; }
            }
            public override Color ToolStripContentPanelGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripContentPanelGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ToolStripGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ToolStripPanelGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ToolStripPanelGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color GripDark
            {
                get { return DarkBG; }
            }
            public override Color GripLight
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color ImageMarginRevealedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color MenuItemPressedGradientMiddle
            {
                get { return DarkBG; }
            }
            public override Color RaftingContainerGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color RaftingContainerGradientEnd
            {
                get { return DarkBG; }
            }
            public override Color StatusStripGradientBegin
            {
                get { return DarkBG; }
            }
            public override Color StatusStripGradientEnd
            {
                get { return DarkBG; }
            }
        }
    }
}
