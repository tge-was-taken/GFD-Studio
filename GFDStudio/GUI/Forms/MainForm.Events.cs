using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Controls;
using GFDStudio.GUI.DataViewNodes;
using GFDStudio.IO;
using Ookii.Dialogs;

namespace GFDStudio.GUI.Forms
{
    public partial class MainForm
    {
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

            if ( handled || ModelEditorTreeView.SelectedNode?.ContextMenuStrip == null )
                return;

            // If it's not a main menu shortcut, try checking if its a shortcut to one of the selected
            // node's context menu actions.
            foreach ( var item in ModelEditorTreeView.SelectedNode.ContextMenuStrip.Items )
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

            var paths = ( string[] )data;
            if ( paths.Length == 0 )
                return;

            var path = paths[0];
            if ( !File.Exists( path ) )
                return;

            OpenFile( path );
        }

        private static void HandleDragEnter( object sender, DragEventArgs e )
        {
            e.Effect = e.Data.GetDataPresent( DataFormats.FileDrop ) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void HandleTreeViewUserPropertyChanged( object sender, DataViewNodePropertyChangedEventArgs args )
        {
            var controls = mContentPanel.Controls.Find( nameof( ModelViewControl ), true );

            if ( controls.Length == 1 )
            {
                var modelViewControl = ( ModelViewControl )controls[0];
                if ( ModelEditorTreeView.TopNode.Data is ModelPack modelPack )
                    modelViewControl.LoadModel( modelPack );
            }
        }


        private void HandleTreeViewAfterSelect( object sender, TreeViewEventArgs e )
        {
            var node = ( DataViewNode )e.Node;
            UpdateSelection( node );
        }

        private void HandleOpenToolStripMenuItemClick( object sender, EventArgs e )
        {
            mFileToolStripMenuItem.DropDown.Close();
            SelectAndOpenSelectedFile();
        }

        private void HandleOpenToolStripRecentlyOpenedFileClick( object sender, EventArgs e )
        {
            mFileToolStripMenuItem.DropDown.Close();
            OpenFile( ( ( ToolStripMenuItem )sender ).Text );
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
            else if ( ModelEditorTreeView.TopNode != null )
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
            if ( ModelEditorTreeView.TopNode == null )
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
                var node = DataViewNodeFactory.Create( "Model", model );
                ModelEditorTreeView.SetTopNode( node );
                LastOpenedFilePath = null;
            }
        }

        private void HandleRetargetAnimationsToolStripMenuItemClick( object sender, EventArgs e )
        {
            var originalScene = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the original model file." )?.Model;
            if ( originalScene == null )
                return;

            var newScene = ModuleImportUtilities.SelectImportFile<ModelPack>( "Select the new model file." )?.Model;
            if ( newScene == null )
                return;

            bool fixArms = MessageBox.Show( "Fix arms? If unsure, select No.", "Question", MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes;

            string directoryPath;
            using ( var dialog = new VistaFolderBrowserDialog() )
            {
                dialog.Description =
                    "Select a directory containing GAP files, or subdirectories containing GAP files to retarget to the new model.\n" +
                    "Note that this will replace the original files.";

                if ( dialog.ShowDialog() != DialogResult.OK )
                    return;

                directoryPath = dialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            using ( var dialog = new ProgressDialog() )
            {
                dialog.DoWork += ( o, progress ) =>
                {
                    var filePaths = Directory.EnumerateFiles( directoryPath, "*.GAP", SearchOption.AllDirectories ).ToList();
                    var processedFileCount = 0;

                    Parallel.ForEach( filePaths, ( filePath, state ) =>
                    {
                        lock ( dialog )
                        {
                            if ( dialog.CancellationPending )
                            {
                                state.Stop();
                                return;
                            }

                            dialog.ReportProgress( ( int )( ( ( float )++processedFileCount / filePaths.Count ) * 100 ),
                                                   $"Processing {Path.GetFileName( filePath )}", null );
                        }

                        try
                        {
                            var animationPack = Resource.Load<AnimationPack>( filePath );
                            animationPack.Retarget( originalScene, newScene, fixArms );
                            animationPack.Save( filePath );
                        }
                        catch ( Exception ex )
                        {
                            failures.Add( filePath );
                        }
                    } );
                };

                dialog.ShowDialog();
            }

            if ( failures.Count > 0 )
            {
                MessageBox.Show( "An error occured while processing the following files:\n" + string.Join( "\n", failures ) );
            }
        }

        private void HandleAnimationLoadExternalToolStripMenuItemClick( object sender, EventArgs e )
        {
            var filePath = SelectFileToOpen( ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import, typeof( AnimationPack ),
                                                                                   typeof( Animation ) ) );
            if ( filePath == null )
                return;

            if ( !DataViewNodeFactory.TryCreate( filePath, out var node ) )
            {
                MessageBox.Show( "Hee file could not be loaded, ho.", "Error", MessageBoxButtons.OK );
                return;
            }

            if ( node.DataType == typeof(Animation) )
            {
                ModelViewControl.Instance.LoadAnimation( ( Animation )node.Data );
            }
            else if ( node.DataType == typeof(AnimationPack) )
            {
                var animationPack = ( AnimationPack ) node.Data;
                var animation     = animationPack.Animations.FirstOrDefault();
                if ( animation != null )
                    ModelViewControl.Instance.LoadAnimation( animation );
            }
            else
            {
                MessageBox.Show( "Not a support animation file.", "Error", MessageBoxButtons.OK );
                return;
            }

            mAnimationListTreeView.SetTopNode( node );
        }

        private void HandleModelAnimationLoaded( object sender, Animation e )
        {
            mAnimationTrackBar.Minimum = -1;
            mAnimationTrackBar.Maximum = ( int ) ( e.Duration * 1000 );
        }

        private void HandleModelAnimationPlaybackStateChanged( object sender, AnimationPlaybackState e )
        {
            switch ( e )
            {
                case AnimationPlaybackState.Stopped:
                case AnimationPlaybackState.Paused:
                    mAnimationPlaybackButton.Text = "Play";
                    break;
            
                case AnimationPlaybackState.Playing:
                    mAnimationPlaybackButton.Text = "Pause";
                    break;
            }
        }

        private double mLastAnimationTime;
        private bool mIgnoreNextTrackBarChange;
        private void HandleModelAnimationTimeChanged( object sender, double e )
        {
            if ( !ModelViewControl.Instance.IsAnimationLoaded )
                return;

            mLastAnimationTime = e;
            var value = e * 1000;
            mIgnoreNextTrackBarChange = true;
            mAnimationTrackBar.Value = ( int ) value;
        }


        private void HandleAnimationPlaybackButtonClick( object sender, EventArgs e )
        {
            if ( !ModelViewControl.Instance.IsAnimationLoaded )
                return;

            switch ( ModelViewControl.Instance.AnimationPlayback )
            {
                case AnimationPlaybackState.Stopped:
                case AnimationPlaybackState.Paused:
                    ModelViewControl.Instance.AnimationPlayback = AnimationPlaybackState.Playing;
                    break;

                case AnimationPlaybackState.Playing:
                    ModelViewControl.Instance.AnimationPlayback = AnimationPlaybackState.Paused;
                    break;
            }
        }

        private void HandleTrackbarValueChanged( object sender, EventArgs e )
        {
            if ( !mIgnoreNextTrackBarChange )
            {
                ModelViewControl.Instance.AnimationTime = mAnimationTrackBar.Value / 1000d;
                ModelViewControl.Instance.Invalidate();
            }
            else
            {
                mIgnoreNextTrackBarChange = false;
            }
        }

        private void HandleAnimationTreeViewAfterSelect( object sender, TreeViewEventArgs e )
        {
            var node = ( DataViewNode )e.Node;
            if ( node.DataType == typeof(Animation) )
                ModelViewControl.Instance.LoadAnimation( ( Animation ) node.Data );

            mPropertyGrid.SelectedObject = node;
        }

        private void HandleAnimationStopButtonClick( object sender, EventArgs e )
        {
            if ( !ModelViewControl.Instance.IsAnimationLoaded )
                return;

            ModelViewControl.Instance.AnimationPlayback = AnimationPlaybackState.Stopped;
            ModelViewControl.Instance.Invalidate();
        }
    }
}
