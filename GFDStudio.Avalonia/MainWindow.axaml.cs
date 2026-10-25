using System;
using Avalonia.Controls;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Models;
using GFDStudio.FormatModules;
using GFDStudio.GUI;
using GFDStudio.GUI.DataViewNodes;
using GFDStudio.IO;

namespace GFDStudio.Avalonia
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mTree.SelectionChanged += ( s, e ) => ShowSelection( mTree.SelectedNode );
            mTree.NodeSelected += ( s, node ) => ShowSelection( node );

            mOpen.Click += ( s, e ) => OpenFile();
            mSave.Click += ( s, e ) => SaveFile();
            mSaveAs.Click += ( s, e ) => SaveFileAs();
            mExit.Click += ( s, e ) => Close();

            BindOption( mRetainTextureNames, x => Options.RetainMaterialTextureNames = x, Options.RetainMaterialTextureNames );
            BindOption( mRetainColorValues, x => Options.RetainMaterialColorValues = x, Options.RetainMaterialColorValues );
            BindOption( mSaveTexturesExternally, x => Options.SaveReplacedTexturesExternally = x, Options.SaveReplacedTexturesExternally );
        }

        private static UserOptionsAvalonia Options => (UserOptionsAvalonia)UserOptions.Current;

        public DataViewNode TopNode { get; private set; }

        public void OpenFile()
        {
            var result = UserDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select a file to open",
                Filter = ModuleFilterGenerator.GenerateFilterForAllSupportedImportFormats(),
            } );

            if ( result != null )
                OpenFile( result.FileName );
        }

        public void OpenFile( string path )
        {
            if ( !DataViewNodeFactory.TryCreate( path, out var node ) )
            {
                UserDialogs.Current.ShowMessage( $"The file could not be loaded:\n{path}", "Error", UserMessageKind.Error );
                return;
            }

            // Several parts of a Metaphor model pack live in files beside it.
            if ( node.DataType == typeof( ModelPack ) && ( (ModelPack)node.Data ).Version >= 0x02000000 )
                CollectModelParts.CollectDisconnectedModelPartsForModelPack( (ModelPack)node.Data, path );

            TopNode = node;
            mTree.Load( node );
            mTree.Select( node );
            mStatus.Text = path;
        }

        public void SaveFile()
        {
            if ( TopNode == null )
                return;

            TopNode.Export();
        }

        public void SaveFileAs()
        {
            if ( TopNode == null )
                return;

            TopNode.Export();
        }

        private static void BindOption( MenuItem item, Action<bool> set, bool initial )
        {
            item.IsChecked = initial;
            item.Click += ( s, e ) => set( item.IsChecked );
        }

        private void ShowSelection( DataViewNode node )
        {
            mProperties.DataContext = node;
            mStatus.Text = node == null ? string.Empty : $"{node.Text}  ({node.DataType.Name})";
        }

    }
}
