using System;
using System.Windows.Forms;
using GFDLibrary;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Controls;
using GFDStudio.GUI.DataViewNodes;

namespace GFDStudio.GUI.Forms
{
    public partial class MainForm : Form
    {
        private static MainForm sInstance;
        public static MainForm Instance
        {
            get => sInstance;
            set
            {
                if ( sInstance == null )
                    sInstance = value;
                else
                    throw new InvalidOperationException();
            }
        }


        private FileHistoryList mFileHistoryList;

        public DataTreeView DataTreeView { get; private set; }

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
            Instance = this;

#if DEBUG
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision} [DEBUG]";
#else
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Revision}";
#endif

            mFileHistoryList = new FileHistoryList( "file_history.txt", 10, mOpenToolStripMenuItem.DropDown.Items,
                                                    HandleOpenToolStripRecentlyOpenedFileClick );
            DataTreeView.LabelEdit = true;
            AllowDrop = true;
        }

        private void InitializeEvents()
        {
            DataTreeView.AfterSelect += HandleTreeViewAfterSelect;
            DataTreeView.UserPropertyChanged += HandleTreeViewUserPropertyChanged;
            DataTreeView.KeyDown += HandleKeyDown;
            mContentPanel.ControlAdded += HandleContentPanelControlAdded;
            mContentPanel.Resize += HandleContentPanelResize;
            DragDrop += HandleDragDrop;
            DragEnter += HandleDragEnter;
        }

        //
        // De-initialization
        //
        private void DeInitialize()
        {
            mFileHistoryList.Save();
        }

        //
        // Recently opened files list
        //
        private void RecordOpenedFile( string filePath )
        {
            LastOpenedFilePath = filePath;
            mFileHistoryList.Add( filePath );
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
            if ( !DataViewNodeFactory.TryCreate( filePath, out var node ) )
            {
                MessageBox.Show( "Hee file could not be loaded, ho.", "Error", MessageBoxButtons.OK );
                return;
            }

            RecordOpenedFile( filePath );
            DataTreeView.SetTopNode( node );
            UpdateSelection( node );
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
            if ( DataTreeView.Nodes.Count > 0 )
                DataTreeView.TopNode.Export( filePath );
        }

        public string SelectFileAndSave()
        {
            if ( DataTreeView.Nodes.Count > 0 )
                return DataTreeView.TopNode.Export();

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

        public void UpdateSelection()
        {
            var selectedNode = ( DataViewNode )mPropertyGrid.SelectedObject;
            if ( selectedNode != null )
                UpdateSelection( selectedNode );
        }

        private void UpdateSelection( DataViewNode node )
        {
            // Set property grid to display properties of the currently selected node
            mPropertyGrid.SelectedObject = node;

            Control control = null;

            if ( FormatModuleRegistry.ModuleByType.TryGetValue( node.DataType, out var module ) )
            {
                if ( module.UsageFlags.HasFlag( FormatModuleUsageFlags.Bitmap ) )
                {
                    control = new BitmapViewControl( module.GetBitmap( node.Data ) );
                    control.Visible = false;
                }
                else if ( module.ModelType == typeof( ModelPack ) )
                {
                    ClearContentPanel();
                    var modelViewControl = new ModelViewControl();
                    modelViewControl.Name = nameof( ModelViewControl );
                    modelViewControl.Visible = false;
                    modelViewControl.LoadModel( ( ModelPack )node.Data );
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

    }
}
