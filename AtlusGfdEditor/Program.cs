using System;
using System.Diagnostics;
using System.Windows.Forms;
using AtlusGfdEditor;
using AtlusGfdEditor.GfdLib;
using AtlusGfdEditor.GfdLib.Internal;

namespace AtlusGfdEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DumpMaterialInfo();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Gui.MainForm());
        }

        static void DumpMaterialInfo()
        {
            //GfdResource.Load(@"D:\Modding\Persona 5\Dump\model\character\0002\c0002_001_00.GMD");
        }
    }
}
