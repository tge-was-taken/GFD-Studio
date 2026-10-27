using System;

namespace GFDStudio.Cli
{
    /// <summary>
    /// An error the user can put right by changing the command line. These are reported as a
    /// single line without a stack trace.
    /// </summary>
    public sealed class CliException : Exception
    {
        public CliException( string message ) : base( message ) { }
    }
}
