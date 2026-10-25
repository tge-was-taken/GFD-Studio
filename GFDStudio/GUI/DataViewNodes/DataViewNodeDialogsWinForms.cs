using System.Windows.Forms;
using Ookii.Dialogs.Wpf;

namespace GFDStudio.GUI.DataViewNodes
{
    /// <summary>
    /// The WinForms prompts behind <see cref="IDataViewNodeDialogs"/>.
    /// </summary>
    public sealed class DataViewNodeDialogsWinForms : IDataViewNodeDialogs
    {
        public void ShowMessage( string text, string caption, DataViewNodeMessageKind kind )
        {
            var icon = kind switch
            {
                DataViewNodeMessageKind.Warning => MessageBoxIcon.Exclamation,
                DataViewNodeMessageKind.Error => MessageBoxIcon.Error,
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
    }
}
