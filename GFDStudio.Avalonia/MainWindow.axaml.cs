using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDLibrary.Models;
using GFDLibrary.Rendering.OpenGL;
using GFDStudio.FormatModules;
using GFDStudio.GUI;
using GFDStudio.GUI.DataViewNodes;
using GFDStudio.IO;

namespace GFDStudio.Avalonia
{
    public partial class MainWindow : Window
    {
        private readonly FileHistory mFileHistory = new( "file_history.txt", 10 );
        private string mFilePath;
        private bool mUpdatingAnimationTime;

        public MainWindow()
        {
            InitializeComponent();

            Title = $"GFD Studio {typeof( MainWindow ).Assembly.GetName().Version?.ToString( 3 )}";

            mTree.SelectionChanged += ( s, e ) => ShowSelection( mTree.SelectedNode );
            mTree.NodeSelected += ( s, node ) => ShowSelection( node );
            mAnimationTree.SelectionChanged += ( s, e ) => SelectAnimation( mAnimationTree.SelectedNode );

            DragDrop.SetAllowDrop( this, true );
            AddHandler( DragDrop.DragOverEvent, OnDragOver );
            AddHandler( DragDrop.DropEvent, OnDrop );

            mNewModel.Click += ( s, e ) => NewModel();
            mOpen.Click += ( s, e ) => OpenFile();
            mSave.Click += ( s, e ) => SaveFile();
            mSaveAs.Click += ( s, e ) => SaveFileAs();
            mExit.Click += ( s, e ) => Close();

            mLoadAnimation.Click += ( s, e ) => LoadExternalAnimation();

            mTools.Click += ( s, e ) => new ToolsWindow().ShowDialog( this );
            mOptions.Click += ( s, e ) => new OptionsWindow( Settings, ApplyTheme ).ShowDialog( this );

            ApplyTheme( Settings.DarkMode );

            mPlayPause.Click += ( s, e ) => TogglePlayback();
            mStop.Click += ( s, e ) => mModelView.AnimationPlayback = AnimationPlaybackState.Stopped;
            mAnimationTime.PropertyChanged += ( s, e ) =>
            {
                if ( e.Property == Slider.ValueProperty && !mUpdatingAnimationTime )
                    mModelView.AnimationTime = mAnimationTime.Value / 1000d;
            };

            mModelView.AnimationLoaded += ( s, animation ) => ShowAnimationControls( animation );
            mModelView.AnimationPlaybackStateChanged += ( s, state ) =>
                mPlayPause.Content = state == AnimationPlaybackState.Playing ? "\u23F8" : "\u25B6";
            mModelView.AnimationTimeChanged += ( s, time ) =>
            {
                mUpdatingAnimationTime = true;
                mAnimationTime.Value = time * 1000d;
                mUpdatingAnimationTime = false;
            };

            foreach ( var toggle in new[] { mChannelR, mChannelG, mChannelB, mChannelA } )
                toggle.IsCheckedChanged += ( s, e ) => UpdateTextureChannels();

            mTextureReset.Click += ( s, e ) => mTextureView.ResetView();

            RebuildRecentFiles();
            Closing += ( s, e ) =>
            {
                mFileHistory.Save();
                Settings.SaveJson( Settings );
            };
        }

        private static Config Settings => UserOptionsAvalonia.Settings;

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

            mFileHistory.Add( path );
            RebuildRecentFiles();

            if ( node.DataType == typeof( Animation ) || node.DataType == typeof( AnimationPack ) )
            {
                mAnimationTree.Load( node );
                mStatus.Text = path;
                return;
            }

            // Several parts of a Metaphor model pack live in files beside it.
            if ( node.DataType == typeof( ModelPack ) && ( (ModelPack)node.Data ).Version >= 0x02000000 )
                CollectModelParts.CollectDisconnectedModelPartsForModelPack( (ModelPack)node.Data, path );

            TopNode = node;
            mFilePath = path;
            mTree.Load( node );
            mTree.Select( node );
            mStatus.Text = path;

            // The tree does not always raise a selection change for the node it was just given,
            // so the preview is asked for directly rather than waiting on one.
            ShowSelection( node );
        }

        public void NewModel()
        {
            var model = ModelConverterUtility.ConvertAssimpModel( null );
            if ( model == null )
                return;

            TopNode = DataViewNodeFactory.Create( "Model", model );
            mFilePath = null;
            mTree.Load( TopNode );
            mTree.Select( TopNode );
            ShowSelection( TopNode );
        }

        /// <summary>
        /// Writes the open file back where it came from.
        /// </summary>
        public void SaveFile()
        {
            if ( TopNode == null )
                return;

            if ( mFilePath == null )
                SaveFileAs();
            else
                TopNode.Export( mFilePath );
        }

        public void SaveFileAs()
        {
            var path = TopNode?.Export();
            if ( path == null )
                return;

            mFilePath = path;
            mFileHistory.Add( path );
            RebuildRecentFiles();
        }

        private void LoadExternalAnimation()
        {
            var result = UserDialogs.Current.OpenFile( new FileDialogRequest
            {
                Title = "Select an animation to load",
                Filter = ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import, typeof( AnimationPack ),
                                                               typeof( Animation ) ).Filter,
            } );

            if ( result == null )
                return;

            if ( !DataViewNodeFactory.TryCreate( result.FileName, out var node ) )
            {
                UserDialogs.Current.ShowMessage( "The file could not be loaded.", "Error", UserMessageKind.Error );
                return;
            }

            if ( node.DataType == typeof( Animation ) )
            {
                mModelView.LoadAnimation( (Animation)node.Data );
            }
            else if ( node.DataType == typeof( AnimationPack ) )
            {
                var animation = ( (AnimationPack)node.Data ).Animations.FirstOrDefault();
                if ( animation != null )
                    mModelView.LoadAnimation( animation );
            }
            else
            {
                UserDialogs.Current.ShowMessage( "Not a supported animation file.", "Error", UserMessageKind.Error );
                return;
            }

            mAnimationTree.Load( node );
        }

        private void SelectAnimation( DataViewNode node )
        {
            if ( node == null )
                return;

            if ( node.DataType == typeof( Animation ) )
                mModelView.LoadAnimation( (Animation)node.Data );

            mProperties.DataContext = node;
        }

        private void TogglePlayback()
            => mModelView.AnimationPlayback = mModelView.AnimationPlayback == AnimationPlaybackState.Playing
                ? AnimationPlaybackState.Paused
                : AnimationPlaybackState.Playing;

        private void ShowAnimationControls( Animation animation )
        {
            mAnimationControls.IsEnabled = animation != null;
            if ( animation == null )
                return;

            mAnimationTime.Maximum = animation.Duration * 1000d;
        }

        private void RebuildRecentFiles()
        {
            var items = mFileHistory.Files.Select( path =>
            {
                var item = new MenuItem { Header = path };
                item.Click += ( s, e ) => OpenFile( path );
                return item;
            } ).ToList();

            mRecent.ItemsSource = items;
            mRecent.IsEnabled = items.Count > 0;
        }

        private void ApplyTheme( bool dark )
        {
            Settings.DarkMode = dark;
            if ( Application.Current != null )
                Application.Current.RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        private static void OnDragOver( object sender, DragEventArgs e )
            => e.DragEffects = e.DataTransfer.Contains( DataFormat.File )
                               ? DragDropEffects.Copy
                               : DragDropEffects.None;

        private void OnDrop( object sender, DragEventArgs e )
        {
            var file = e.DataTransfer.TryGetFile()?.TryGetLocalPath();
            if ( file != null && File.Exists( file ) )
                OpenFile( file );
        }

        private void ShowSelection( DataViewNode node )
        {
            mProperties.DataContext = node;
            mStatus.Text = node == null ? string.Empty : $"{node.Text}  ({node.DataType.Name})";

            ShowPreview( node );
        }

        /// <summary>
        /// Shows whichever preview suits the node. A node with nothing of its own to show leaves
        /// the model up, and picks itself out in it where it is part of one.
        /// </summary>
        private void ShowPreview( DataViewNode node )
        {
            if ( node == null )
                return;

            if ( FormatModuleRegistry.ModuleByType.TryGetValue( node.DataType, out var module ) )
            {
                if ( module.UsageFlags.HasFlag( FormatModuleUsageFlags.Image ) )
                {
                    var image = module.GetImage( node.Data );
                    mTextureView.Show( image );
                    mTextureSize.Text = $"{image.Width} x {image.Height}";
                    mTexturePanel.IsVisible = true;
                    mModelView.IsVisible = false;
                    return;
                }

                if ( module.ModelType == typeof( ModelPack ) )
                {
                    mModelView.LoadModel( (ModelPack)node.Data );
                    mModelView.IsVisible = true;
                    mTexturePanel.IsVisible = false;
                    return;
                }
            }

            HighlightInModel( node );
        }

        private void HighlightInModel( DataViewNode node )
        {
            if ( !mModelView.HasModel )
                return;

            switch ( node.Data )
            {
                case Mesh mesh:
                    mModelView.SetSelection( mesh );
                    break;

                case Material material:
                    mModelView.SetSelection( material );
                    break;

                default:
                    mModelView.ClearSelection();
                    break;
            }

            mModelView.IsVisible = true;
            mTexturePanel.IsVisible = false;
        }

        private void UpdateTextureChannels()
        {
            var channels = TextureChannels.None;

            if ( mChannelR.IsChecked == true ) channels |= TextureChannels.Red;
            if ( mChannelG.IsChecked == true ) channels |= TextureChannels.Green;
            if ( mChannelB.IsChecked == true ) channels |= TextureChannels.Blue;
            if ( mChannelA.IsChecked == true ) channels |= TextureChannels.Alpha;

            mTextureView.Channels = channels;
        }
    }
}
