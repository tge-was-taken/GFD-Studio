using System;

namespace GFDStudio.GUI.DataViewNodes
{
    public enum DataViewNodeMessageKind
    {
        Information,
        Warning,
        Error,
    }

    public sealed record FileDialogRequest
    {
        public string Title { get; init; }

        /// <summary>
        /// A filter in the form the format modules generate, which is the WinForms syntax of
        /// description and patterns separated by pipes.
        /// </summary>
        public string Filter { get; init; }

        public string FileName { get; init; }

        public bool Multiselect { get; init; }
    }

    /// <summary>
    /// The outcome of a file dialog, or null when the user cancelled.
    /// </summary>
    /// <param name="FileNames">The chosen paths, never empty.</param>
    /// <param name="FilterIndex">The chosen filter, counted from one as the format modules expect.</param>
    public sealed record FileDialogResult( string[] FileNames, int FilterIndex )
    {
        public string FileName => FileNames[0];
    }

    /// <summary>
    /// The prompts a view node needs. Implemented once per UI framework so the nodes do not open
    /// windows themselves.
    /// </summary>
    public interface IDataViewNodeDialogs
    {
        void ShowMessage( string text, string caption, DataViewNodeMessageKind kind );

        bool AskYesNo( string text, string caption, bool defaultYes = false );

        FileDialogResult OpenFile( FileDialogRequest request );

        FileDialogResult SaveFile( FileDialogRequest request );

        /// <summary>
        /// Asks for a directory, returning null when the user cancelled.
        /// </summary>
        string SelectFolder( string title );
    }

    /// <summary>
    /// The dialogs the view nodes use, set once by the application that hosts them.
    /// </summary>
    public static class DataViewNodeDialogs
    {
        private static IDataViewNodeDialogs mCurrent;

        public static IDataViewNodeDialogs Current
        {
            get => mCurrent ?? throw new InvalidOperationException( "No dialog implementation has been registered" );
            set => mCurrent = value;
        }
    }
}
