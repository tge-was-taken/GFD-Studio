using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GFDLibrary.Conversion;
using GFDLibrary;
using Ookii.Dialogs.Wpf;

namespace GFDStudio.GUI.Forms
{
    /// <summary>
    /// The WinForms prompts behind <see cref="IUserDialogs"/>.
    /// </summary>
    public sealed class UserDialogsWinForms : IUserDialogs
    {
        public void ShowMessage( string text, string caption, UserMessageKind kind )
        {
            var icon = kind switch
            {
                UserMessageKind.Warning => MessageBoxIcon.Exclamation,
                UserMessageKind.Error => MessageBoxIcon.Error,
                _ => MessageBoxIcon.Information,
            };

            MessageBox.Show( text, caption, MessageBoxButtons.OK, icon );
        }

        public bool AskYesNo( string text, string caption, bool defaultYes = false )
            => MessageBox.Show( text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                                defaultYes ? MessageBoxDefaultButton.Button1 : MessageBoxDefaultButton.Button2 )
               == DialogResult.Yes;

        public FileDialogResult OpenFile( FileDialogRequest request )
        {
            using var dialog = new OpenFileDialog
            {
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CheckFileExists = true,
                FileName = request.FileName,
                Filter = request.Filter,
                Multiselect = request.Multiselect,
                SupportMultiDottedExtensions = true,
                Title = request.Title,
                ValidateNames = true,
            };

            return dialog.ShowDialog() != DialogResult.OK
                ? null
                : new FileDialogResult( dialog.FileNames, dialog.FilterIndex );
        }

        public string SelectFolder( string title )
        {
            var dialog = new VistaFolderBrowserDialog { Description = title, UseDescriptionForTitle = true };
            return dialog.ShowDialog() == true ? dialog.SelectedPath : null;
        }

        public FileDialogResult SaveFile( FileDialogRequest request )
        {
            using var dialog = new SaveFileDialog
            {
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                FileName = request.FileName,
                Filter = request.Filter,
                OverwritePrompt = true,
                Title = request.Title,
                ValidateNames = true,
                AddExtension = true,
            };

            return dialog.ShowDialog() != DialogResult.OK
                ? null
                : new FileDialogResult( dialog.FileNames, dialog.FilterIndex );
        }

        public ModelConverterOptions AskModelConversionOptions( Assimp.Scene scene, ModelPack originalModel )
        {
            using var dialog = new ModelConversionOptionsDialog( scene, originalModel );
            return dialog.ShowDialog() != DialogResult.OK ? null : dialog.GetModelConversionOptions();
        }

        public TransformRequest AskTransform( string title )
        {
            using var dialog = new SetScaleValueDialog();
            if ( dialog.ShowDialog() != DialogResult.OK )
                return null;

            return new TransformRequest( dialog.Result.Scale, dialog.Result.Position, dialog.Result.Rotation );
        }

        public void RunWithProgress( string title, Func<IProgressReport, Task> work )
        {
            using var dialog = new ProgressDialog { WindowTitle = title };
            dialog.DoWork += async ( s, e ) => await work( new ProgressReport( dialog ) );
            dialog.ShowDialog();
        }

        private sealed class ProgressReport : IProgressReport
        {
            private readonly ProgressDialog mDialog;

            public ProgressReport( ProgressDialog dialog ) => mDialog = dialog;

            public bool CancellationPending => mDialog.CancellationPending;

            public void Report( int percentage, string message )
            {
                lock ( mDialog )
                    mDialog.ReportProgress( percentage, message, null );
            }
        }

        public NewTextureMap AskNewTextureMap()
        {
            using var dialog = new CreateTextureMapDialog();
            return dialog.ShowDialog() != DialogResult.OK
                ? null
                : new NewTextureMap( dialog.Result.Name, dialog.Result.Type );
        }
    }
}
