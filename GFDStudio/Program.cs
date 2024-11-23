﻿using System;
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

        public static bool DarkMode { get; set; } = true;

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
                try
                {
                    File.AppendAllTextAsync( "GFDStudio.log", fmt );
                }
                catch ( Exception )
                {
                    // C'est la vie
                }
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            using (var form = new ModelConversionOptionsDialog())
                Application.Run( form );

            //using ( var mainForm = new MainForm() )
            //{
            //    if ( args.Length > 0 && File.Exists( args[0] ) )
            //        mainForm.OpenFile( args[0] );

            //    Application.Run( mainForm );
            //}
        }

#if WINDOWS
        [DllImport( "kernel32.dll", SetLastError = true )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool AllocConsole();
#endif
    }
}
