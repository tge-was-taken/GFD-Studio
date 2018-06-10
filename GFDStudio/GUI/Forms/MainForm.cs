using System;
using System.IO;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Controls;
using GFDStudio.GUI.ViewModels;
using GFDStudio.IO;

namespace GFDStudio.GUI.Forms
{
    public partial class MainForm : Form
    {
        private const string RECENTLY_OPENED_FILES_LIST_FILEPATH = "RecentlyOpenedFiles.txt";

        private RecentlyOpenedFilesList mRecentlyOpenedFilesList;

        public TreeNodeViewModelView TreeView { get; private set; }

        public string LastOpenedFilePath { get; private set; }

        //
        // Initialization
        //
        public MainForm()
        {
            InitializeComponent();
            InitializeState();
            InitializeEvents();
        }

        private void InitializeState()
        {

#if DEBUG
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision} [DEBUG]";
#else
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision}";
#endif

            mRecentlyOpenedFilesList = new RecentlyOpenedFilesList( RECENTLY_OPENED_FILES_LIST_FILEPATH, 10, mOpenToolStripMenuItem.DropDown.Items,
                                                                    HandleOpenToolStripRecentlyOpenedFileClick );
            TreeView.LabelEdit = true;
            AllowDrop = true;
        }

        private void InitializeEvents()
        {
            TreeView.AfterSelect += HandleTreeViewAfterSelect;
            TreeView.UserPropertyChanged += HandleTreeViewUserPropertyChanged;
            TreeView.KeyDown += HandleKeyDown;
            mContentPanel.ControlAdded += HandleContentPanelControlAdded;
            mContentPanel.Resize += HandleContentPanelResize;
            DragDrop += HandleDragDrop;
            DragEnter += HandleDragEnter;
        }

        private void HandleKeyDown( object sender, KeyEventArgs e )
        {
            if ( e.Control )
                HandleControlShortcuts( e.KeyData );
        }

        private void HandleControlShortcuts( Keys keys )
        {
            var handled = false;

            // Check main menu strip shortcuts first
            foreach ( var item in MainMenuStrip.Items )
            {
                var menuItem = item as ToolStripMenuItem;
                if ( menuItem?.ShortcutKeys == keys )
                {
                    menuItem.PerformClick();
                    handled = true;
                }
            }

            if ( handled || TreeView.SelectedNode?.ContextMenuStrip == null )
                return;

            // If it's not a main menu shortcut, try checking if its a shortcut to one of the selected
            // node's context menu actions.
            foreach ( var item in TreeView.SelectedNode.ContextMenuStrip.Items )
            {
                var menuItem = item as ToolStripMenuItem;
                if ( menuItem?.ShortcutKeys == keys )
                    menuItem.PerformClick();
            }
        }

        private void HandleDragDrop( object sender, DragEventArgs e )
        {
            var data = e.Data.GetData( DataFormats.FileDrop );
            if ( data == null )
                return;

            var paths = ( string[] ) data;
            if ( paths.Length == 0 )
                return;

            var path = paths[ 0 ];
            if ( !File.Exists( path ) )
                return;

            OpenFile( path );
        }

        private static void HandleDragEnter( object sender, DragEventArgs e )
        {
            e.Effect = e.Data.GetDataPresent( DataFormats.FileDrop ) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void HandleTreeViewUserPropertyChanged( object sender, TreeNodeViewModelPropertyChangedEventArgs args )
        {
            var controls = mContentPanel.Controls.Find( nameof( ModelViewControl ), true );

            if ( controls.Length == 1 )
            {
                var modelViewControl = ( ModelViewControl )controls[ 0 ];
                modelViewControl.LoadModel( ( Model ) TreeView.TopNode.Model );
            }
        }

        //
        // De-initialization
        //
        private void DeInitialize()
        {
            mRecentlyOpenedFilesList.Save( RECENTLY_OPENED_FILES_LIST_FILEPATH );
        }

        //
        // Recently opened files list
        //
        private void RecordOpenedFile( string filePath )
        {
            LastOpenedFilePath = filePath;
            mRecentlyOpenedFilesList.Add( filePath );
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
                dialog.Filter = ModuleFilterGenerator.GenerateFilterForAllSupportedImportFormats();
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                return dialog.FileName;
            }
        }

        public void OpenFile( string filePath )
        {
            if ( !TreeNodeViewModelFactory.TryCreate( filePath, out var viewModel ) )
            {
                MessageBox.Show( "Hee file could not be loaded, ho.", "Error", MessageBoxButtons.OK );
                return;
            }

            RecordOpenedFile( filePath );
            TreeView.SetTopNode( viewModel );
            OnTreeNodeViewModelSelected( viewModel );
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
            if ( TreeView.Nodes.Count > 0 )
                TreeView.TopNode.Export( filePath );
        }

        public string SelectFileAndSave()
        {
            if ( TreeView.Nodes.Count > 0 )
                return TreeView.TopNode.Export();

            return null;
        }

        private void ClearContentPanel()
        {
            foreach ( var control in mContentPanel.Controls )
            {
                ( control as IDisposable )?.Dispose();
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

        private void HandleTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        {
            var viewModel = ( TreeNodeViewModel )e.Node;
            OnTreeNodeViewModelSelected( viewModel );
        }

        private void OnTreeNodeViewModelSelected( TreeNodeViewModel viewModel )
        {
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
                else if ( module.ModelType == typeof( Model ) )
                {
                    ClearContentPanel();
                    var modelViewControl = new ModelViewControl();
                    modelViewControl.Name = nameof( ModelViewControl );
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
        }

        private void HandleOpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            mFileToolStripMenuItem.DropDown.Close();
            SelectAndOpenSelectedFile();
        }

        private void HandleOpenToolStripRecentlyOpenedFileClick( object sender, EventArgs e )
        {
            mFileToolStripMenuItem.DropDown.Close();
            OpenFile( (( ToolStripMenuItem )sender).Text );
        }

        private void HandleContentPanelControlAdded( object sender, ControlEventArgs e )
        {
            e.Control.Left = ( mContentPanel.Width - e.Control.Width ) / 2;
            e.Control.Top = ( mContentPanel.Height - e.Control.Height ) / 2;
            e.Control.Visible = true;
        }

        private void HandleContentPanelResize( object sender, EventArgs e )
        {
            foreach ( Control control in mContentPanel.Controls )
            {
                control.Visible = false;
                control.Left = ( mContentPanel.Width - control.Width ) / 2;
                control.Top = ( mContentPanel.Height - control.Height ) / 2;
                control.Visible = true;
            }
        }

        private void HandleSaveToolStripMenuItemClick( object sender, EventArgs e )
        {
            if ( LastOpenedFilePath != null )
            {
                SaveFile( LastOpenedFilePath );
            }
            else if ( TreeView.TopNode != null )
            {
                MessageBox.Show( "No hee file opened, ho! Use the 'Save as...' option instead.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            else
            {
                MessageBox.Show( "Nothing to save, hee ho!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void HandleSaveAsToolStripMenuItemClick( object sender, EventArgs e )
        {
            if ( TreeView.TopNode == null )
            {
                MessageBox.Show( "Nothing to save, hee ho!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            var path = SelectFileAndSave();
            if ( path != null )
            {
                OpenFile( path );
            }
        }

        private void HandleNewModelToolStripMenuItemClick( object sender, EventArgs e )
        {
            var model = ModelConverterUtility.ConvertAssimpModel();
            if ( model != null )
            {
                var viewModel = TreeNodeViewModelFactory.Create( "Model", model );
                TreeView.SetTopNode( viewModel );
                LastOpenedFilePath = null;
            }
        }

    }
}
