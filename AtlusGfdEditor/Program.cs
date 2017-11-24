using System;
using System.Reflection;
using System.Windows.Forms;
using AtlusGfdEditor.GUI;
using AtlusGfdEditor.GUI.Forms;

namespace AtlusGfdEditor
{
    internal static class Program
    {
        public static Assembly     Assembly     = Assembly.GetExecutingAssembly();
        public static AssemblyName AssemblyName = Assembly.GetName();
        public static string       Name         = AssemblyName.Name;
        public static Version      Version      = AssemblyName.Version;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
