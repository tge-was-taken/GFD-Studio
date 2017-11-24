using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using AtlusGfdEditor.FormatModules;
using AtlusGfdEditor.GUI.Controls;
using AtlusGfdEditor.GUI.ViewModels;
using AtlusGfdLib;

namespace AtlusGfdEditor.GUI.Forms
{
    public partial class MainForm : Form
    {
        private TreeNodeViewModel mLastSelectedNode;
        private Stack<string> mRecentlyOpenedFileHistoryStack;
        private const string RECENTLY_OPENED_FILES_LIST_FILEPATH = "RecentlyOpenedFiles.txt";

        public TreeNodeViewModelView TreeView => mTreeView;

        //
        // Initialization
        //
        public MainForm()
        {
            InitializeComponent();
            InitializeState();
            InitializeRecentlyOpenedFilesList();
            InitializeEvents();
        }

        private void InitializeState()
        {

#if DEBUG
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision} [DEBUG]";
#else
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision}";
#endif

            mTreeView.LabelEdit = true;
            
        }

        private void InitializeRecentlyOpenedFilesList()
        {
            mRecentlyOpenedFileHistoryStack = new Stack<string>();

            if ( File.Exists( RECENTLY_OPENED_FILES_LIST_FILEPATH ) )
            {
                foreach ( string line in File.ReadAllLines( RECENTLY_OPENED_FILES_LIST_FILEPATH ) )
                {
                    AddRecentlyOpenedFile( line );
                }
            }
        }

        private void InitializeEvents()
        {
            mTreeView.AfterSelect += TreeViewAfterSelectEventHandler;
            mContentPanel.ControlAdded += ContentPanelControlAddedEventHandler;
            mContentPanel.Resize += ContentPanelResizeEventHandler;
        }

        //
        // De-initialization
        //
        private void DeInitialize()
        {
            SaveRecentlyOpenedFilesList();
        }

        //
        // Recently opened files list
        //
        private void AddRecentlyOpenedFile( string filePath )
        {
            mRecentlyOpenedFileHistoryStack.Push( filePath );

            var item = new ToolStripMenuItem( filePath );
            item.Click += OpenToolStripRecentlyOpenedFileClickEventHandler;

            mOpenToolStripMenuItem.DropDown.Items.Insert( 0, item );
        }

        public string GetLastOpenedFile()
        {
            if ( mRecentlyOpenedFileHistoryStack.Count == 0 )
                return null;

            return mRecentlyOpenedFileHistoryStack.Peek();
        }

        private void SaveRecentlyOpenedFilesList()
        {
            using ( var writer = File.CreateText( RECENTLY_OPENED_FILES_LIST_FILEPATH ) )
            {
                var paths = mRecentlyOpenedFileHistoryStack.ToArray().Reverse();
                foreach ( var path in paths )
                {
                    writer.WriteLine( path );
                }
            }
        }

        //
        // File IO
        //
        public string SelectAndOpenSelectedFile()
        {
            var filePath = SelectFileToOpen();
            if ( filePath != null )
                OpenFile( filePath );

            return filePath;
        }

        public string SelectFileToOpen()
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import );
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                return dialog.FileName;
            }
        }

        public void OpenFile( string filePath )
        {
            if ( !TreeNodeViewModelFactory.TryCreate( filePath, out var viewModel ) )
            {
                MessageBox.Show( "File could not be loaded.", "Error", MessageBoxButtons.OK );
                return;
            }

            AddRecentlyOpenedFile( filePath );

            mTreeView.SetTopNode( viewModel );
        }

        public string SelectFileToSaveTo()
        {
            using ( var dialog = new SaveFileDialog() )
            {
                dialog.Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Export );
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                return dialog.FileName;
            }
        }

        public void SaveFile( string filePath )
        {
            if ( mTreeView != null && mTreeView.TopNode != null )
            {
                mTreeView.TopNode.Export( filePath );
            }
        }

        public string SelectFileAndSave()
        {
            if ( mTreeView != null && mTreeView.TopNode != null )
            {
                return mTreeView.TopNode.Export();
            }

            return null;
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

        //
        // Event handlers
        //
        protected override void OnClosed( EventArgs e )
        {
            DeInitialize();

            // exit application when the main form is closed
            Application.Exit();
        }

        private void TreeViewAfterSelectEventHandler(object sender, TreeViewEventArgs e)
        {
            var viewModel = ( TreeNodeViewModel )e.Node;

            // Set property grid to display properties of the currently selected node
            mPropertyGrid.SelectedObject = viewModel;

            Control control = null;

            if ( FormatModuleRegistry.ModuleByType.TryGetValue( viewModel.ModelType, out var module ) )
            {
                if ( module.UsageFlags.HasFlag( FormatModuleUsageFlags.Bitmap ) )
                {
                    control = new BitmapViewControl( module.GetBitmap( viewModel.Model ) );
                    control.Visible = false;
                }
                else if ( module.ModelType == typeof(Model) )
                {
                    ClearContentPanel();
                    var modelViewControl = new ModelViewControl();
                    modelViewControl.Visible = false;
                    modelViewControl.LoadModel( ( Model )viewModel.Model );
                    control = modelViewControl;
                }
            }

            if ( control != null )
            {
                // Clear the content panel
                ClearContentPanel();
                mContentPanel.Controls.Add( control );
            }

            mLastSelectedNode = viewModel;
        }

        private void OpenToolStripMenuItemClickEventHandler(object sender, EventArgs e)
        {
            mFileToolStripMenuItem.DropDown.Close();
            SelectAndOpenSelectedFile();
        }

        private void OpenToolStripRecentlyOpenedFileClickEventHandler( object sender, EventArgs e )
        {
            mFileToolStripMenuItem.DropDown.Close();
            OpenFile( (( ToolStripMenuItem )sender).Text );
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

        private void SaveToolStripMenuItemClickEventHandler( object sender, EventArgs e )
        {
            SaveFile( GetLastOpenedFile() );
        }

        private void SaveAsToolStripMenuItemClickEventHandler( object sender, EventArgs e )
        {
            var path = SelectFileAndSave();
            OpenFile( path );
        }
    }
}
