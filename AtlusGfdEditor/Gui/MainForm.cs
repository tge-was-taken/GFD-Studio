using System;
using System.Windows.Forms;
using AtlusGfdLib;
using AtlusGfdEditor.Modules;
using AtlusGfdEditor.GUI.Adapters;
using System.IO;
using AtlusGfdEditor.GUI.Controls;

namespace AtlusGfdEditor.GUI
{
    public partial class MainForm : Form
    {
        private TreeNodeAdapter mLastSelectedNode;

        public TreeNodeAdapterView TreeView => mTreeView;

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
            mContentPanel.ControlAdded += ContentPanelControlAddedEventHandler;
            mContentPanel.Resize += ContentPanelResizeEventHandler;
        }

        private void ClearContentPanel()
        {
            foreach ( var control in mContentPanel.Controls )
            {
                if ( control is IDisposable )
                {
                    ( ( IDisposable )control ).Dispose();
                }
            }

            mContentPanel.Controls.Clear();
        }

        // Event handlers
        protected override void OnClosed( EventArgs e )
        {
            // exit application when the main form is closed
            Application.Exit();
        }

        private void TreeViewAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            var adapter = ( TreeNodeAdapter )e.Node;
            if ( adapter == mLastSelectedNode )
                return;

            // Set property grid to display properties of the currently selected node
            mPropertyGrid.SelectedObject = adapter;

            // Clear the content panel
            ClearContentPanel();

            if ( ModuleRegistry.ModuleByType.TryGetValue( adapter.ResourceType, out var module ) )
            {
                if ( module.UsageFlags.HasFlag( FormatModuleUsageFlags.Bitmap ) )
                {
                    var control = new BitmapViewControl( module.GetBitmap( adapter.Resource ) );
                    control.Visible = false;

                    mContentPanel.Controls.Add( control );
                }
                else if ( module.ObjectType == typeof(Model) )
                {
                    var control = new ModelViewControl();
                    control.Visible = false;
                    control.LoadModel( ( Model )adapter.Resource );

                    mContentPanel.Controls.Add( control );
                }
            }

            mLastSelectedNode = adapter;
        }

        private void OpenToolStripMenuItemClickEventHandler(object sender, EventArgs e)
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import );
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return;

                if ( !TreeNodeAdapterFactory.TryCreate( dialog.FileName, out var adapter ) )
                {
                    MessageBox.Show( "File could not be loaded.", "Error", MessageBoxButtons.OK );
                    return;
                }

                mTreeView.SetTopNode( adapter );
            }
        }

        private void ContentPanelControlAddedEventHandler( object sender, ControlEventArgs e )
        {
            e.Control.Left = ( mContentPanel.Width - e.Control.Width ) / 2;
            e.Control.Top = ( mContentPanel.Height - e.Control.Height ) / 2;
            e.Control.Visible = true;
        }

        private void ContentPanelResizeEventHandler( object sender, EventArgs e )
        {
            foreach ( Control control in mContentPanel.Controls )
            {
                control.Visible = false;
                control.Left = ( mContentPanel.Width - control.Width ) / 2;
                control.Top = ( mContentPanel.Height - control.Height ) / 2;
                control.Visible = true;
            }
        }
    }
}
