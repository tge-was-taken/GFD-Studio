using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using GFDLibrary;
using GFDLibrary.Animations;
using GFDLibrary.Materials;
using GFDStudio.FormatModules;
using GFDStudio.GUI.Controls;
using GFDStudio.GUI.DataViewNodes;
using GFDStudio.IO;
using GFDLibrary.Textures;
using Ookii.Dialogs.Wpf;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Windows.Markup;
using GFDLibrary.Conversion;

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
            var browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory containing GAP files, or subdirectories containing GAP files to retarget to the new model.\n" +
                    "Note that this will replace the original files.";

                if ( browserDialog.ShowDialog() != true )
                    return;

                directoryPath = browserDialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            using ( var dialog = new ProgressDialog() )
            {
                dialog.WindowTitle = "Retargeting Animations";
                dialog.DoWork += async ( o, progress ) =>
                {
                    var filePaths = Directory.EnumerateFiles( directoryPath, "*.GAP", SearchOption.AllDirectories ).ToList();
                    var processedFileCount = 0;

                    var tasks = filePaths.Select( async filePath =>
                    {
                        lock ( dialog )
                        {
                            if ( dialog.CancellationPending )
                                return;
                            dialog.ReportProgress( (int)( ( (float)Interlocked.Increment( ref processedFileCount ) / filePaths.Count ) * 100 ),
                                                   $"Processing {Path.GetFileName( filePath )}", null );
                        }

                        try
                        {
                            await Task.Run( () =>
                            {
                                var animationPack = Resource.Load<AnimationPack>( filePath );
                                animationPack.Retarget( originalScene, newScene, fixArms );
                                animationPack.Save( filePath );
                            } );
                        }
                        catch ( Exception )
                        {
                            failures.Add( filePath );
                        }
                    } );

                    await Task.WhenAll( tasks );
                    // Retargeting Metaphor anim packs in particular is quite memory intensive
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    if ( !failures.IsEmpty )
                    {
                        MessageBox.Show( "An error occured while processing the following files:\n" + string.Join( "\n", failures ) );
                    }
                    else MessageBox.Show( "All animation packs successfully retargeted!" );
                };

                dialog.ShowDialog();
            }
        }
		
        private void HandleConvertAnimationsToolStripMenuItemClick(object sender, EventArgs e)
        {
            string directoryPath;
            var folderDIalog = new VistaFolderBrowserDialog();
            {
                folderDIalog.Description =
                    "Select a directory containing GAP files, or subdirectories containing GAP files to convert the animations.\n" +
                    "Note that this will replace the original files.";

                if (folderDIalog.ShowDialog() != true)
                    return;

                directoryPath = folderDIalog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            using ( var dialog = new ProgressDialog() )
            {
                dialog.DoWork += async ( o, progress ) =>
                {
                    var filePaths = Directory.EnumerateFiles( directoryPath, "*.GAP", SearchOption.AllDirectories ).ToList();
                    var processedFileCount = 0;

                    var tasks = filePaths.Select( async filePath =>
                    {
                        lock ( dialog )
                        {
                            if ( dialog.CancellationPending )
                                return;
                            dialog.ReportProgress( (int)( ( (float)Interlocked.Increment( ref processedFileCount ) / filePaths.Count ) * 100 ),
                                                   $"Processing {Path.GetFileName( filePath )}", null );
                        }

                        try
                        {
                            await Task.Run( () =>
                            {
                                var animationPack = Resource.Load<AnimationPack>( filePath );
                                animationPack.ConvertToP5();
                                animationPack.Save( filePath );
                            } );
                        }
                        catch ( Exception )
                        {
                            failures.Add( filePath );
                        }
                    } );

                    await Task.WhenAll( tasks );
                };

                dialog.ShowDialog();
            }

            if (failures.Count > 0)
            {
                MessageBox.Show("An error occured while processing the following files:\n" + string.Join("\n", failures));
            }
            else MessageBox.Show( "All animation packs successfully converted!" );
        }

        private void HandleConvertMaterialsToolStripMenuItemClick(object sender, EventArgs e)
        {
            string directoryPath;
            var folderDialog = new VistaFolderBrowserDialog();
            {
                folderDialog.Description =
                    "Select a directory containing GMD files, or subdirectories containing GMD files to convert the materials.\n" +
                    "Note that this will replace the original files.";

                if (folderDialog.ShowDialog() != true)
                    return;

                directoryPath = folderDialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            ModelConverterOptions option;
            using (var dialog = new ModelConverterOptionsDialog(false))
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                ModelConverterOptions options = new ModelConverterOptions()
                {
                    MaterialPreset = dialog.MaterialPreset,
                    Version = dialog.Version
                };

                option = options;
            }

            using (var dialog = new ProgressDialog())
            {
                dialog.DoWork += (o, progress) =>
                {
                    var filePaths = Directory.EnumerateFiles(directoryPath, "*.GMD", SearchOption.AllDirectories).ToList();
                    var processedFileCount = 0;

                    Parallel.ForEach(filePaths, (filePath, state) =>
                    {
                        lock (dialog)
                        {
                            if (dialog.CancellationPending)
                            {
                                state.Stop();
                                return;
                            }

                            dialog.ReportProgress((int)(((float)++processedFileCount / filePaths.Count) * 100),
                                                   $"Processing {Path.GetFileName(filePath)}", null);
                        }

                        try
                        {
                            var model = Resource.Load<ModelPack>(filePath);
                            var materials = model.Materials;
                            var materialsConverted = MaterialDictionary.ConvertAllToMaterialPreset(materials, option);
                            model.Materials = materialsConverted;
                            model.Save(filePath);
                        }
                        catch (Exception)
                        {
                            failures.Add(filePath);
                        }
                    });
                };

                dialog.ShowDialog();
            }

            if (failures.Count > 0)
            {
                MessageBox.Show("An error occured while processing the following files:\n" + string.Join("\n", failures));
            }
        }

        private void copyP5SplitGAPToMultipleModelsInDirectoryToolStripMenuItem_Click( object sender, EventArgs e )
        {
            var aGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with a." );
            if ( aGap == null )
                return;

            var bGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with b." );
            if ( bGap == null )
                return;

            var cGap = ModuleImportUtilities.SelectImportFile<AnimationPack>( "Select the gap file ending with c." );
            if ( cGap == null )
                return;

            bool isGAP52 = MessageBox.Show( "GAP ID 52? If unsure, select No.", "Question", MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2 ) == DialogResult.Yes;

            string directoryPath;
            var browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory containing Model files, or subdirectories containing model files to make the new GAP files for.\n" +
                    "Note that this will replace the original files.";

                if ( browserDialog.ShowDialog() != true )
                    return;

                directoryPath = browserDialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            string GAP_ID = "_051_";

            if ( isGAP52 )
            {
                GAP_ID = "_052_";
            }    

            var filePaths = Directory.EnumerateFiles( directoryPath, "*.GMD", SearchOption.AllDirectories ).ToList();
            foreach ( string filePath in filePaths )
            {
                var targetCharID = Path.GetFileName( filePath ).Split( "_" )[0].Remove( 0, 1 ); // get character ID from model
                var targetGAPname = Path.GetFileName( filePath ).Split( "_" )[1]; // get model ID from model
                var targetGAPDir = Path.Join( Path.GetDirectoryName( filePath ), "battle"); // directory of model
                try
                {
                    var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath )?.Model;

                    if ( targetModel == null )
                        continue;

                    aGap.FixTargetIds( targetModel );
                    aGap.Save( Path.Join( targetGAPDir, "bb" + targetCharID + GAP_ID + targetGAPname + "a.GAP" ));

                    bGap.FixTargetIds( targetModel );
                    bGap.Save( Path.Join( targetGAPDir, "bb" + targetCharID + GAP_ID + targetGAPname + "b.GAP" ) );

                    cGap.FixTargetIds( targetModel );
                    cGap.Save( Path.Join( targetGAPDir, "bb" + targetCharID + GAP_ID + targetGAPname + "c.GAP" ) );
                }
                catch ( Exception )
                {
                    failures.Add( filePath );
                }
            }

            if ( failures.Count > 0 )
            {
                MessageBox.Show( "An error occured while processing the following files:\n" + string.Join( "\n", failures ) );
            }
            else MessageBox.Show( "All split GAPs successfully generated!" );
        }
        private void MassExportTexturesToolStripMenuItem_Click( object sender, EventArgs e )
        {
            string directoryPath;
            var browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory containing Model files to extract textures from.\n";

                if ( browserDialog.ShowDialog() != true )
                    return;

                directoryPath = browserDialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            string outpath;
            browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory to dump all textures into.\n";

                if ( browserDialog.ShowDialog() != true )
                    return;

                outpath = browserDialog.SelectedPath;
            }

            string[] formats = { ".GFS", ".GMD" };

            foreach ( var filePath in Directory.EnumerateFiles( directoryPath, "*.*", SearchOption.AllDirectories ).Where( x => formats.Any( x.EndsWith ) ) )
            {
                try
                {
                    var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath );
                    if ( targetModel.Version >= 0x2000000 )
                        CollectModelParts.CollectDisconnectedModelPartsForModelPack( targetModel, filePath );
                    var targetModelTexDic = targetModel.Textures;
                    if ( targetModelTexDic == null )
                        continue;

                    foreach ( var tex in targetModelTexDic.Textures )
                    {
                        string texPath;

                        switch ( tex.Format )
                        {
                            case TextureFormat.DDS:
                                texPath = Path.Combine( outpath, Path.ChangeExtension( tex.Name, ".dds" ) );
                                File.WriteAllBytes( texPath, tex.Data );
                                break;
                            case TextureFormat.TGA:
                                texPath = Path.Combine( outpath, tex.Name );
                                File.WriteAllBytes( texPath, tex.Data );
                                break;
                            case TextureFormat.GNF:
                                texPath = Path.Combine( outpath, Path.ChangeExtension( tex.Name, ".gnf" ) );
                                File.WriteAllBytes( texPath, tex.Data );
                                break;
                            case TextureFormat.TMX:
                                texPath = Path.Combine( outpath, tex.Name );
                                File.WriteAllBytes( texPath, tex.Data );
                                break;
                            default:
                                texPath = Path.Combine( outpath, Path.ChangeExtension( tex.Name, ".png" ) );
                                TextureDecoder.Decode( tex ).Save( texPath );
                                break;
                        }
                    }
                }
                catch ( Exception )
                {
                    failures.Add( filePath );
                }
            }

            if ( failures.Count > 0 )
            {
                MessageBox.Show( "An error occured while processing the following files:\n" + string.Join( "\n", failures ) );
            }
            else MessageBox.Show( "All Textures successfully mass dumped!" );
        }

        public Texture HandleTextureToReplace( uint ver, Texture tex )
        {
            tex.Version = ver;
            if ( settings.SaveReplacedTexturesExternally && tex.Version >= 0x2000000 )
                tex.IsTexSourceExternal = true;
            return tex;
        }
        private void MassReplaceTexturesToolStripMenuItem_Click( object sender, EventArgs e )
        {
            string directoryPath;
            var browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory containing Model files to replace textures in.\n";

                if ( browserDialog.ShowDialog() != true )
                    return;

                directoryPath = browserDialog.SelectedPath;
            }

            var failures = new ConcurrentBag<string>();

            string TexReplaceDir;
            browserDialog = new VistaFolderBrowserDialog();
            {
                browserDialog.Description =
                    "Select a directory containing the textures you wish to replace in models.\n";

                if ( browserDialog.ShowDialog() != true )
                    return;

                TexReplaceDir = browserDialog.SelectedPath;
            }
            string[] formats = { ".GFS", ".GMD" };

            string GetTextureToReplacePath(uint version, string texpath)
                => (version >= 0x2000000) ? Path.GetFileNameWithoutExtension(texpath) : Path.GetFileName( texpath );
            string GetOriginalTextureNamePath(uint version, string name )
                => ( version >= 0x2000000 ) ? Path.GetFileNameWithoutExtension(name) : name;

            var TargetTextures = Directory.EnumerateFiles( TexReplaceDir, "*.dds", SearchOption.AllDirectories ).ToList();

            foreach ( var filePath in Directory.EnumerateFiles( directoryPath, "*.*", SearchOption.AllDirectories ).Where( x => formats.Any( x.EndsWith ) ) )
            {
                try
                {
                    var targetModel = ModuleImportUtilities.ImportFile<ModelPack>( filePath );
                    if ( targetModel == null )
                        continue;

                    var targetModelTexDic = targetModel.Textures;
                    if ( targetModelTexDic == null )
                        continue;

                    var newTexDic = new TextureDictionary( targetModel.Version );
                    foreach ( var tex in targetModelTexDic.Textures )
                    {
                        string OriginalTexName = GetOriginalTextureNamePath( targetModel.Version, tex.Name );
                        Logger.Debug( $"Tex Replace: For model {filePath}, try replacing {OriginalTexName}" );
                        string FoundTexPath = null;
                        foreach ( var replaceTex in TargetTextures )
                        {
                            string ReplaceTexName = GetTextureToReplacePath( targetModel.Version, replaceTex );
                            if ( ReplaceTexName == OriginalTexName )
                            {
                                FoundTexPath = replaceTex;
                                break;
                            }
                        }
                        if ( FoundTexPath != null )
                        {
                            Logger.Debug( $"Tex Replace: replacing texture with { FoundTexPath }" );
                            Texture newTexture = HandleTextureToReplace( targetModel.Version, new Texture( tex.Name, TextureFormat.DDS, File.ReadAllBytes( FoundTexPath ) ) );
                            newTexDic.Add( newTexture );
                        }
                        else newTexDic.Add( tex );
                    }

                    targetModel.Textures.ReplaceWith( newTexDic );
                    targetModel.Save( filePath );
                    Logger.Debug( $"Tex Replace: saved model to {filePath}" );
                }
                catch ( Exception )
                {
                    failures.Add( filePath );
                }
            }

            if ( failures.Count > 0 )
            {
                MessageBox.Show( "An error occured while processing the following files:\n" + string.Join( "\n", failures ) );
            }
            else MessageBox.Show( "All Textures successfully mass replaced in models!" );
        }

        private void HandleRescaleAnimationsToolStripMenuItemClick( object sender, EventArgs e)
        {
            string directoryPath;
            var folderDialog = new VistaFolderBrowserDialog();
            {
                folderDialog.Description =
                    "Select a directory containing GAP files, or subdirectories containing GAP files to rescale.\n" +
                    "Note that this will replace the original files.";

                if (folderDialog.ShowDialog() != true)
                    return;

                directoryPath = folderDialog.SelectedPath;
            }

            Vector3 scale;
            Vector3 position;
            Quaternion rotation;
            using (var scaleDialog = new SetScaleValueDialog())
            {
                if (scaleDialog.ShowDialog() != DialogResult.OK)
                    return;
                scale = scaleDialog.Result.Scale;
                position = scaleDialog.Result.Position;
                rotation = scaleDialog.Result.Rotation;
            }

            var failures = new ConcurrentBag<string>();

            using ( var dialog = new ProgressDialog() )
            {
                dialog.DoWork += async ( o, progress ) =>
                {
                    var filePaths = Directory.EnumerateFiles( directoryPath, "*.GAP", SearchOption.AllDirectories ).ToList();
                    var processedFileCount = 0;

                    var tasks = filePaths.Select( async filePath =>
                    {
                        lock ( dialog )
                        {
                            if ( dialog.CancellationPending )
                                return;
                            dialog.ReportProgress( (int)( ( (float)Interlocked.Increment( ref processedFileCount ) / filePaths.Count ) * 100 ),
                                                   $"Processing {Path.GetFileName( filePath )}", null );
                        }

                        try
                        {
                            await Task.Run( () =>
                            {
                                var animationPack = Resource.Load<AnimationPack>( filePath );
                                animationPack.Rescale( scale, position, rotation );
                                animationPack.Save( filePath );
                            } );
                        }
                        catch ( Exception )
                        {
                            failures.Add( filePath );
                        }
                    } );

                    await Task.WhenAll( tasks );
                };

                dialog.ShowDialog();
            }

            if (failures.Count > 0)
            {
                MessageBox.Show("An error occured while processing the following files:\n" + string.Join("\n", failures));
            }
            else MessageBox.Show( "All animation packs successfully rescaled!" );
        }

        private void HandleAnimationLoadExternalToolStripMenuItemClick( object sender, EventArgs e )
        {
            var filePath = SelectFileToOpen( ModuleFilterGenerator.GenerateFilter( FormatModuleUsageFlags.Import, typeof( AnimationPack ),
                                                                                   typeof( Animation ) ).Filter );
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


        private void handleRetainColorCheckedChanged( object sender, EventArgs e )
        {
            settings.RetainMaterialColors = retainColorValuesToolStripMenuItem.Checked;
            settings.SaveJson( settings );
        }

        private void handleRetainTextureCheckedChanged( object sender, EventArgs e )
        {
            settings.RetainTextureNames = retainTexNameToolStripMenuItem.Checked;
            settings.SaveJson( settings );
        }

        private void handleDarkThemeCheckedChanged( object sender, EventArgs e )
        {
            // Toggle setting state and save to .json
            settings.DarkMode = useDarkThemeToolStripMenuItem.Checked;
            settings.SaveJson( settings );

            // Change appearance of form elements
            Theme.Apply( this );
        }

        private void handleSaveReplacedTexturesExternally( object sender, EventArgs e )
        {
            settings.SaveReplacedTexturesExternally = metaphorSaveReplacedTexturesExternallyToolStripMenuItem.Checked;
            settings.SaveJson( settings );
        }
    }
}
