using GFDLibrary.Rendering.OpenGL;
using MetroSet_UI.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GFDStudio.GUI.Forms
{
    public partial class MainForm : MetroSetForm
    {
        public void ToggleTheme()
        {
            var style = MetroSet_UI.Enums.Style.Dark;
            if ( Program.DarkMode )
            {
                mMainMenuStrip.Renderer = new DarkMenuRenderer();
                GUI.Controls.ModelViewControl.Instance.GridLineColor = new Vector4( 50.15f, 50.15f, 50.15f, 1f ).ToOpenTK();
                GUI.Controls.ModelViewControl.Instance.ClearColor = System.Drawing.Color.FromArgb( 60, 63, 65 );
            }
            else
            {
                style = MetroSet_UI.Enums.Style.Light;
                mMainMenuStrip.Renderer = new ToolStripProfessionalRenderer();
                GUI.Controls.ModelViewControl.Instance.GridLineColor = new Vector4( 0.15f, 0.15f, 0.15f, 1f ).ToOpenTK();
                GUI.Controls.ModelViewControl.Instance.ClearColor = System.Drawing.Color.LightGray;
            }
            GUI.Controls.ModelViewControl.Instance.Refresh();

            this.Style = style;

            foreach ( var control in EnumerateControls( this ) )
            {
                dynamic ctrl = control as dynamic;
                if ( PropertyExists( ctrl, "Style" ) )
                    ctrl.Style = style;

                if ( Program.DarkMode )
                {
                    ctrl.BackColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
                    ctrl.ForeColor = System.Drawing.Color.FromArgb( 220, 220, 220 );
                }
                else
                {
                    ctrl.BackColor = System.Drawing.Color.FromArgb( 240, 240, 240 );
                    ctrl.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
                }

                if ( ctrl.GetType() == typeof( MenuStrip ) )
                {
                    foreach ( ToolStripMenuItem item in ctrl.Items )
                    {
                        if ( Program.DarkMode )
                            item.ForeColor = System.Drawing.Color.FromArgb( 220, 220, 220 );
                        else
                            item.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );

                        foreach ( ToolStripMenuItem subItem in item.DropDownItems )
                        {
                            if ( Program.DarkMode )
                                subItem.ForeColor = System.Drawing.Color.FromArgb( 220, 220, 220 );
                            else
                                subItem.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );

                            foreach ( ToolStripMenuItem subSubItem in subItem.DropDownItems )
                            {
                                if ( Program.DarkMode )
                                    subSubItem.ForeColor = System.Drawing.Color.FromArgb( 220, 220, 220 );
                                else
                                    subSubItem.ForeColor = System.Drawing.Color.FromArgb( 30, 30, 30 );
                            }
                        }
                    }
                }
            }

        }

        private void DarkTheme_CheckedChanged( object sender, EventArgs e )
        {
            // Toggle setting state
            if ( Program.DarkMode )
                Program.DarkMode = false;
            else
                Program.DarkMode = true;

            // Manage file used to keep track of previous setting state
            if ( Program.DarkMode && !File.Exists( "UseDarkMode.txt" ) )
                try { File.CreateText( "UseDarkMode.txt" ); } catch { }
            else if ( File.Exists( "UseDarkMode.txt" ) )
                try { File.Delete( "UseDarkMode.txt" ); } catch { }

            // Change appearance of form elements
            ToggleTheme();
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
    }
}
