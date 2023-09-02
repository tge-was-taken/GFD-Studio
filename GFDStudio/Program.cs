using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.GUI.Forms;

namespace GFDStudio
{
    internal static class Program
    {
        public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static AssemblyName AssemblyName { get; } = Assembly.GetName();
        public static string Name { get; } = "GFD Studio";
        public static Version Version { get; } = AssemblyName.Version;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main( string[] args )
        {
#if WINDOWS && DEBUG
            AllocConsole();
#endif
            Logger.Log += ( s, e ) =>
            {
                var fmt = $"{DateTime.Now} {e.Severity} {e.Message}\n";
#if WINDOWS && DEBUG
                Console.WriteLine( fmt );
#endif
                File.AppendAllTextAsync( "GFDStudio.log", fmt );
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            using ( var mainForm = new MainForm() )
            {
                if ( args.Length > 0 && File.Exists( args[0] ) )
                    mainForm.OpenFile( args[0] );

                Application.Run( mainForm );
            }
        }

#if WINDOWS
        [DllImport( "kernel32.dll", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool AllocConsole();
#endif
    }

    public class CustomMenuRenderer : ToolStripProfessionalRenderer
    {
        public CustomMenuRenderer() : base( new CustomColors() ) { }
    }
    public class CustomColors : ProfessionalColorTable
    {
        public override Color MenuItemSelected
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuItemBorder
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuBorder
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color SeparatorDark
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color SeparatorLight
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuStripGradientBegin
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color MenuStripGradientEnd
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
        public override Color ToolStripBorder
        {
            get { return Color.FromArgb( 20, 20, 20 ); }
        }
    }
}
