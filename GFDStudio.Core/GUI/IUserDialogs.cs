using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
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
    /// One way a node can be written out. A null <paramref name="Type"/> means YAML, which is
    /// handled by the view node itself rather than by a format module.
    /// </summary>
    public sealed record ExportFormat( string Name, string Extension, Type Type )
    {
        public override string ToString() => $"{Name}  (*.{Extension})";
    }

    public sealed record ExportOptionsRequest( string SuggestedName, IReadOnlyList<ExportFormat> Formats );

    /// <summary>
    /// Where and how to write the node out, or null when the user cancelled.
    /// </summary>
    public sealed record ExportOptionsResult( string FilePath, ExportFormat Format );

    /// <summary>
    /// A transform the user typed in, or null when they cancelled.
    /// </summary>
    public sealed record TransformRequest( Vector3 Scale, Vector3 Position, Quaternion Rotation );

    /// <summary>
    /// What a long running job reports back to whatever is showing its progress.
    /// </summary>
    public interface IProgressReport
    {
        bool CancellationPending { get; }

        void Report( int percentage, string message );
    }

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

        /// <summary>
        /// Asks where to write a node and in which format, returning null when the user
        /// cancelled.
        /// </summary>
        ExportOptionsResult AskExportOptions( ExportOptionsRequest request );

        /// <summary>
        /// Asks for a scale, position and rotation, returning null when the user cancelled.
        /// </summary>
        TransformRequest AskTransform( string title );

        /// <summary>
        /// Runs a job that takes a while, showing its progress, and returns once it is done.
        /// </summary>
        void RunWithProgress( string title, Func<IProgressReport, Task> work );
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
