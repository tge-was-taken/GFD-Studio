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
                                                                    OpenToolStripRecentlyOpenedFileClickEventHandler );
            TreeView.LabelEdit = true;
            AllowDrop = true;
        }

        private void InitializeEvents()
        {
            TreeView.AfterSelect += TreeViewAfterSelectEventHandler;
            TreeView.UserPropertyChanged += TreeViewUserPropertyChangedEventHandler;
            mContentPanel.ControlAdded += ContentPanelControlAddedEventHandler;
            mContentPanel.Resize += ContentPanelResizeEventHandler;
            DragDrop += DragDropEventHandler;
            DragEnter += DragEnterEventHandler;
        }

        private void DragDropEventHandler( object sender, DragEventArgs e )
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


        private void DragEnterEventHandler( object sender, DragEventArgs e )
        {
            if ( e.Data.GetDataPresent( DataFormats.FileDrop ) )
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void TreeViewUserPropertyChangedEventHandler( object sender, TreeNodeViewModelPropertyChangedEventArgs args )
        {
            var controls = mContentPanel.Controls.Find( nameof( ModelViewControl ), true );

            if ( controls.Length == 1 )
            {
                var modelViewControl = ( ModelViewControl )controls[ 0 ];
                modelViewControl.LoadModel( ( Model ) ( ( TreeNodeViewModel ) TreeView.Nodes[ 0 ] ).Model );
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
                ( ( TreeNodeViewModel )TreeView.Nodes[0] ).Export( filePath );
        }

        public string SelectFileAndSave()
        {
            if ( TreeView.Nodes.Count > 0 )
                return ( ( TreeNodeViewModel )TreeView.Nodes[0] ).Export();

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

        private void TreeViewAfterSelectEventHandler(object sender, TreeViewEventArgs e)
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

        private void SaveAsToolStripMenuItemClickEventHandler( object sender, EventArgs e )
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

        private void NewModelToolStripMenuItemClickEventHandler( object sender, EventArgs e )
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
