using System;
using System.Reflection;
using System.Windows.Forms;
using GFDStudio.GUI.Forms;

namespace GFDStudio
{
    internal static class Program
    {
        public static Assembly     Assembly     { get; } = Assembly.GetExecutingAssembly();
        public static AssemblyName AssemblyName { get; } = Assembly.GetName();
        public static string       Name         { get; } = "GFD Studio";
        public static Version      Version      { get; } = AssemblyName.Version;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
