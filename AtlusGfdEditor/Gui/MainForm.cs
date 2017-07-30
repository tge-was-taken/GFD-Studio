using System;
using System.Windows.Forms;
using AtlusGfdLib;
using AtlusGfdEditor.FormatIOModules;

namespace AtlusGfdEditor.Gui
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeState();
            InitializeEvents();
        }

        private void InitializeState()
        {

#if DEBUG
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Build} [DEBUG]";
#else
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Build}";
#endif

            mTreeView.LabelEdit = true;
        }

        private void InitializeEvents()
        {
            mTreeView.AfterSelect += TreeViewAfterSelectEventHandler;
        }

        // Event handlers
        protected override void OnClosed( EventArgs e )
        {
            // exit application when the main form is closed
            Application.Exit();
        }

        private void TreeViewAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            // Set property grid to display properties of the currently selected node
            mPropertyGrid.SelectedObject = e.Node;
        }

        private void OpenToolStripMenuItemClickEventHandler(object sender, EventArgs e)
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter = FormatIOModuleManager.GenerateFileFilter( FormatModuleUsageFlags.Import );
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return;

                if ( !FormatIOModuleManager.TryGetModuleForImport( dialog.FileName, out var module ) )
                {
                    MessageBox.Show( "File could not be loaded.", "Error", MessageBoxButtons.OK );
                    return;
                }

                // Clear nodes before loading anything
                mTreeView.Nodes.Clear();
            }
        }
    }
}
