using System;
using GFDLibrary.Conversion;
using GFDLibrary;

namespace GFDStudio.GUI
{
    public enum UserMessageKind
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
    /// The texture map the user asked for, or null when they cancelled. A type of -1 means no
    /// slot was picked.
    /// </summary>
    public sealed record NewTextureMap( string Name, int Type );

    /// <summary>
    /// The prompts the application needs. Implemented once per UI framework so that the view
    /// nodes, format modules and converters do not open windows themselves.
    /// </summary>
    public interface IUserDialogs
    {
        void ShowMessage( string text, string caption, UserMessageKind kind );

        bool AskYesNo( string text, string caption, bool defaultYes = false );

        FileDialogResult OpenFile( FileDialogRequest request );

        FileDialogResult SaveFile( FileDialogRequest request );

        /// <summary>
        /// Asks for a directory, returning null when the user cancelled.
        /// </summary>
        string SelectFolder( string title );

        /// <summary>
        /// Asks how to convert an imported scene, returning null when the user cancelled. Both
        /// arguments may be null, which asks for the defaults alone.
        /// </summary>
        ModelConverterOptions AskModelConversionOptions( Assimp.Scene scene, ModelPack originalModel );

        NewTextureMap AskNewTextureMap();
    }

    /// <summary>
    /// The dialogs the application uses, set once by whatever hosts it.
    /// </summary>
    public static class UserDialogs
    {
        private static IUserDialogs mCurrent;

        public static IUserDialogs Current
        {
            get => mCurrent ?? throw new InvalidOperationException( "No dialog implementation has been registered" );
            set => mCurrent = value;
        }
    }
}
