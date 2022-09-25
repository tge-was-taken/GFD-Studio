using System;
using System.Diagnostics;

namespace GFDLibrary
{
    public enum LogSeverity
    {
        Debug,
        Info
    }

    public class LogEventArgs
    {
        public LogSeverity Severity { get; set; }
        public string Message { get; set; }
    }

    public static class Logger
    {
        private static int sIndent = 0;
        private static string sPrefix = "";
        public static EventHandler<LogEventArgs> Log;

        [Conditional("DEBUG")]
        public static void Debug( string message )
        {
            LogMessage( LogSeverity.Debug, message );
        }

        public static void Info( string message )
        {
            LogMessage( LogSeverity.Info, message );
        }

        public static void LogMessage( LogSeverity severity, string message )
        {
            Log?.Invoke( null, new LogEventArgs() { Severity = severity, Message = sPrefix + message } );
        }

        public static void Indent()
        {
            sIndent++;
            sPrefix = new string( '\t', sIndent );
        }

        public static void Unindent()
        {
            sIndent--;
            sPrefix = new string( '\t', sIndent );
        }
    }
}
