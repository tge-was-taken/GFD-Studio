﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Controls;
using GFDStudio.GUI.DataViewNodes;
using MetroSet_UI.Controls;
using MetroSet_UI.Forms;
using Control = System.Windows.Forms.Control;

namespace GFDStudio.GUI.Forms
{
    public partial class MainForm : MetroSet_UI.Forms.MetroSetForm
    {
        public static Config settings = new Config();

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

        public DataTreeView ModelEditorTreeView
        {
            get => mModelEditorTreeView;
            private set => mModelEditorTreeView = value;
        }

        public string LastOpenedFilePath { get; private set; }

        //
        // Initialization
        //
        public MainForm()
        {
            InitializeComponent();
            InitializeState();
            InitializeEvents();

            settings = settings.LoadJson();
            Theme.Apply( this );
            retainColorValuesToolStripMenuItem.Checked = settings.RetainMaterialColors;
            retainTexNameToolStripMenuItem.Checked = settings.RetainTextureNames;
            useDarkThemeToolStripMenuItem.Checked = settings.DarkMode;
#if DEBUG
            //ModelViewControl.Instance.LoadAnimation( Resource.Load<AnimationPack>( 
            //    @"D:\Modding\Persona 5 EU\Main game\ExtractedClean\data\model\character\0001\field\bf0001_002.GAP" ).Animations[2]);
#endif
        }

        private void InitializeState()
        {
            Instance = this;

#if DEBUG
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Build} [DEBUG]";
#else
            Text = $"{Program.Name} {Program.Version.Major}.{Program.Version.Minor}.{Program.Version.Build}";
#endif

            mFileHistoryList = new FileHistoryList( "file_history.txt", 10, mOpenToolStripMenuItem.DropDown.Items,
                                                    HandleOpenToolStripRecentlyOpenedFileClick );
            ModelEditorTreeView.LabelEdit = true;
            AllowDrop = true;
        }

        private void InitializeEvents()
        {
            ModelEditorTreeView.AfterSelect += HandleTreeViewAfterSelect;
            ModelEditorTreeView.UserPropertyChanged += HandleTreeViewUserPropertyChanged;
            ModelEditorTreeView.KeyDown += HandleKeyDown;

            mAnimationListTreeView.AfterSelect += HandleAnimationTreeViewAfterSelect;

            mContentPanel.ControlAdded += HandleContentPanelControlAdded;
            mContentPanel.Resize += HandleContentPanelResize;
            DragDrop += HandleDragDrop;
            DragEnter += HandleDragEnter;
            ModelViewControl.Instance.AnimationLoaded += HandleModelAnimationLoaded;
            ModelViewControl.Instance.AnimationPlaybackStateChanged += HandleModelAnimationPlaybackStateChanged;
            ModelViewControl.Instance.AnimationTimeChanged += HandleModelAnimationTimeChanged;
            mAnimationTrackBar.ValueChanged += HandleTrackbarValueChanged;
            mAnimationPlaybackButton.Click += HandleAnimationPlaybackButtonClick;
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
            return SelectFileToOpen( ModuleFilterGenerator.GenerateFilterForAllSupportedImportFormats() );
        }

        public string SelectFileToOpen( string filter )
        {
            using ( var dialog = new OpenFileDialog() )
            {
                dialog.Filter = filter;
                if ( dialog.ShowDialog() != DialogResult.OK )
                    return null;

                return dialog.FileName;
            }
        }

        public void OpenFile( string filePath )
        {
            Logger.Debug( $"MainForm: Open file {filePath}" );
            if ( !DataViewNodeFactory.TryCreate( filePath, out var node ) )
            {
                MessageBox.Show( "Hee file could not be loaded, ho.", "Error", MessageBoxButtons.OK );
                return;
            }

            RecordOpenedFile( filePath );
            
            if (node.DataType == typeof(Animation) || node.DataType == typeof(AnimationPack))
            {
                mAnimationListTreeView.SetTopNode( node );
            }
            else
            {
                ModelEditorTreeView.SetTopNode( node );
            }

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
            if ( ModelEditorTreeView.Nodes.Count > 0 )
                ModelEditorTreeView.TopNode.Export( filePath );
        }

        public string SelectFileAndSave()
        {
            if ( ModelEditorTreeView.Nodes.Count > 0 )
                return ModelEditorTreeView.TopNode.Export();

            return null;
        }

        private void ClearContentPanel()
        {
            //foreach ( var control in mContentPanel.Controls )
            //{
            //    ( control as IDisposable )?.Dispose();
            //}

            mContentPanel.Controls.Clear();
        }

        //
        // Event handlers
        //
        protected override void OnClosed( EventArgs e )
        {
            mFileHistoryList.Save();

            // exit application when the main form is closed
            Application.Exit();
        }

        public void UpdateSelection()
        {
            var selectedNode = (DataViewNode)mPropertyGrid.SelectedObject;
            if ( selectedNode != null )
                UpdateSelection( selectedNode );
        }

        private void UpdateSelection( DataViewNode node )
        {
            // Set property grid to display properties of the currently selected node
            mPropertyGrid.SelectedObject = node;

            System.Windows.Forms.Control control = null;

            if ( FormatModuleRegistry.ModuleByType.TryGetValue( node.DataType, out var module ) )
            {
                if ( module.UsageFlags.HasFlag( FormatModuleUsageFlags.Bitmap ) )
                {
                    BitmapViewControl.Instance.LoadBitmap( module.GetBitmap( node.Data ) );
                    BitmapViewControl.Instance.Visible = false;
                    control = BitmapViewControl.Instance;
                }
                else if ( module.ModelType == typeof( ModelPack ) )
                {
                    ModelViewControl.Instance.LoadModel( (ModelPack)node.Data );
                    ModelViewControl.Instance.Visible = false;
                    control = ModelViewControl.Instance;
                }
                else if ( node.DataType == typeof( Animation ) )
                {
                    ModelViewControl.Instance.LoadAnimation( (Animation)node.Data );
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
