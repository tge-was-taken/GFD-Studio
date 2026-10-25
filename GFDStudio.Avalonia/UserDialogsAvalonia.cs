using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using GFDLibrary;
using GFDLibrary.Conversion;
using GFDStudio.GUI;

namespace GFDStudio.Avalonia
{
    /// <summary>
    /// The Avalonia prompts behind <see cref="IUserDialogs"/>.
    /// </summary>
    public sealed class UserDialogsAvalonia : IUserDialogs
    {
        private readonly Window mOwner;

        public UserDialogsAvalonia( Window owner ) => mOwner = owner;

        public void ShowMessage( string text, string caption, UserMessageKind kind )
            => Wait( MessageWindow.Show( mOwner, text, caption, MessageWindowButtons.Ok ) );

        public bool AskYesNo( string text, string caption, bool defaultYes = false )
            => Wait( MessageWindow.Show( mOwner, text, caption, MessageWindowButtons.YesNo ) );

        public FileDialogResult OpenFile( FileDialogRequest request )
        {
            var filters = FileFilter.Parse( request.Filter );

            var files = Wait( mOwner.StorageProvider.OpenFilePickerAsync( new FilePickerOpenOptions
            {
                Title = request.Title,
                AllowMultiple = request.Multiselect,
                FileTypeFilter = filters.Select( x => x.ToPickerType() ).ToList(),
            } ) );

            var paths = LocalPaths( files );
            return paths.Length == 0 ? null : new FileDialogResult( paths, FileFilter.IndexOf( filters, paths[0] ) );
        }

        public FileDialogResult SaveFile( FileDialogRequest request )
        {
            var filters = FileFilter.Parse( request.Filter );

            var file = Wait( mOwner.StorageProvider.SaveFilePickerAsync( new FilePickerSaveOptions
            {
                Title = request.Title,
                SuggestedFileName = request.FileName,
                FileTypeChoices = filters.Select( x => x.ToPickerType() ).ToList(),
                ShowOverwritePrompt = true,
            } ) );

            var paths = LocalPaths( file == null ? [] : [file] );
            return paths.Length == 0 ? null : new FileDialogResult( paths, FileFilter.IndexOf( filters, paths[0] ) );
        }

        public string SelectFolder( string title )
        {
            var folders = Wait( mOwner.StorageProvider.OpenFolderPickerAsync( new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
            } ) );

            return LocalPaths( folders ).FirstOrDefault();
        }

        public ModelConverterOptions AskModelConversionOptions( Assimp.Scene scene, ModelPack originalModel )
            => Wait( ModelConversionOptionsWindow.Show( mOwner, scene, originalModel ) );

        public TransformRequest AskTransform( string title )
            => Wait( TransformWindow.Ask( mOwner, title ) );

        public void RunWithProgress( string title, Func<IProgressReport, Task> work )
            => Wait( ProgressWindow.Run( mOwner, title, work ) );

        public NewTextureMap AskNewTextureMap()
            => Wait( NewTextureMapWindow.Ask( mOwner ) );

        private static string[] LocalPaths( IEnumerable<IStorageItem> items )
            => items?.Select( x => x.TryGetLocalPath() ).Where( x => x != null ).ToArray() ?? [];

        /// <summary>
        /// Runs a picker to completion without returning to the caller, which is what the
        /// synchronous callers in the view nodes expect of a modal dialog. The nested dispatcher
        /// frame keeps the window responsive in the meantime, the same way a modal loop does.
        /// </summary>
        private static void Wait( Task task )
        {
            if ( !task.IsCompleted )
            {
                var frame = new DispatcherFrame();
                task.ContinueWith( _ => frame.Continue = false, TaskScheduler.FromCurrentSynchronizationContext() );
                Dispatcher.UIThread.PushFrame( frame );
            }

            task.GetAwaiter().GetResult();
        }

        private static T Wait<T>( Task<T> task )
        {
            if ( !task.IsCompleted )
            {
                var frame = new DispatcherFrame();
                task.ContinueWith( _ => frame.Continue = false, TaskScheduler.FromCurrentSynchronizationContext() );
                Dispatcher.UIThread.PushFrame( frame );
            }

            return task.GetAwaiter().GetResult();
        }
    }
}
